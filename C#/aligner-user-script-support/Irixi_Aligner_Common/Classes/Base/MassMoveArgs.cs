using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Irixi_Aligner_Common.Classes.Base
{
    [Serializable]
    public class MassMoveArgs : ObservableCollection<AxisMoveArgs>
    {
        #region Variables
        int[] moveOrderList = null;
        #endregion

        #region Constructors

        public MassMoveArgs()
        {

        }

        public MassMoveArgs(IEnumerable<AxisMoveArgs> Collection) : base(Collection)
        {
            CreateMoveOrderList();

            foreach (var item in this)
            {
                ((AxisMoveArgs)item).Container = this;
            }
        }
        
        #endregion


        #region Properties

        public string LogicalMotionComponent { get; set; }

        /// <summary>
        /// Get the hash string calculated in realtime
        /// the property is used for the write method of json converter object
        /// </summary>
        public string HashString
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(LogicalMotionComponent);

                foreach (var axisArgs in this)
                {
                    sb.Append(axisArgs.HashString);
                }

                return HashGenerator.GetHashSHA256(sb.ToString());
            }
        }

        /// <summary>
        /// Get the hash string saved in the preset profile.
        /// the property is used for the read
        /// </summary>
        public string HashStringInPresetProfile { private get; set; }

        public int[] MoveOrderList
        {
            private set
            {
                moveOrderList = value;
                OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs("MoveOrderList"));
                //UpdateProperty(ref moveOrderList, value);
            }
            get
            {
                return moveOrderList;
            }
        }

        #endregion

        #region Methods
        
        private void CreateMoveOrderList()
        {
            int[] list = new int[this.Count];
            if (this.Count == 0)
                this.MoveOrderList = null;
            else
            {
                for (int i = 0; i < this.Count; i++)
                    list[i] = i + 1;
                this.MoveOrderList = list;
            }
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            CreateMoveOrderList();

            if(e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach(var item in e.NewItems)
                {
                    ((AxisMoveArgs)item).Container = this;
                }
            }

            base.OnCollectionChanged(e);
        }

        public int[] GetDistinctMoveOrder()
        {
            if (this.Count <= 0)
                return null;
            else
            {
                var ret = this.GroupBy(arg => arg.MoveOrder).Select(grp => grp.First().MoveOrder).OrderBy(i => i);
                return ret.ToArray();
            }
        }

        /// <summary>
        /// Convert the json string to the instance of MassMoveArgs
        /// </summary>
        /// <param name="JsonString"></param>
        /// <returns></returns>
        public static MassMoveArgs FromJsonString(string JsonString)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new MassMoveArgsJsonConverter());
            var args = JsonConvert.DeserializeObject<MassMoveArgs>(JsonString, new MassMoveArgsJsonConverter());

            // check if the preset profile was modified
            if (args.HashString != args.HashStringInPresetProfile)
            {
                throw new FormatException("the preset profile might be modified unexpectedly.");
            }
            else
            {
                return args;
            }
        }

        /// <summary>
        /// Convert the instance of MassMoveArgs to json string
        /// </summary>
        /// <param name="Arg">The instance of MassMovrArgs object</param>
        /// <returns></returns>
        public static string ToJsonString(MassMoveArgs Arg)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.Converters.Add(new MassMoveArgsJsonConverter());
            var json = JsonConvert.SerializeObject(Arg, settings);
            return json;
        }

        #endregion
    }


    /// <summary>
    /// The customized converter to serialize and deserialize the MassMoveArgs
    /// <see cref="https://www.jerriepelser.com/blog/custom-converters-in-json-net-case-study-1/"/>
    /// </summary>
    internal class MassMoveArgsJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(MassMoveArgs).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var inst = value as MassMoveArgs;
            IEnumerable<AxisMoveArgs> array = (IEnumerable<AxisMoveArgs>)value;

            writer.WriteStartObject();
            writer.WritePropertyName("LogicalMotionComponent");
            writer.WriteValue(inst.LogicalMotionComponent);
            writer.WritePropertyName("HashString");
            writer.WriteValue(inst.HashString);
            writer.WritePropertyName("Args");
            writer.WriteStartArray();
            foreach (var item in array)
            {
                serializer.Serialize(writer, item);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject item = JObject.Load(reader);
                if (item["Args"] != null)
                {
                    var argsList = item["Args"].ToObject<IEnumerable<AxisMoveArgs>>(serializer);
                    var obj = new MassMoveArgs(argsList)
                    {
                        LogicalMotionComponent = item["LogicalMotionComponent"].Value<string>(),
                        HashStringInPresetProfile = item["HashString"].Value<string>()
                    };

                    return obj;
                }
                else
                {
                    throw new JsonReaderException("the move args array can not be null.");
                }
            }
            else
            {
                throw new JsonReaderException("the token type is not StartObject.");
            }
        }
    }
}
