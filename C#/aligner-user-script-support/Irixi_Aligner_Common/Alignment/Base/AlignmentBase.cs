using Irixi_Aligner_Common.Classes.Base;
using Irixi_Aligner_Common.Interfaces;
using System;
using System.Threading;

namespace Irixi_Aligner_Common.Alignment.Base
{
    public class AlignmentBase : IAlignmentHandler
    {
        #region Variables

        protected CancellationTokenSource cts;
        protected CancellationToken cts_token;

        protected DateTime alignStarts;

        #endregion

        #region Constructors

        public AlignmentBase(AlignmentArgsBase Args)
        {
            this.Args = Args;
            this.Log = new ObservableCollectionThreadSafe<string>();
        }

        #endregion

        #region Properties

        public IAlignmentArgs Args { get; }

        /// <summary>
        /// Get the messages of the alignment process
        /// </summary>
        public ObservableCollectionThreadSafe<string> Log { get; }

        #endregion



        protected void AddLog(string Message)
        {
            Log.Add(Message);
        }

        public virtual void Start()
        {
            cts = new CancellationTokenSource();
            cts_token = cts.Token;

            Args.Validate();

            Args.ClearScanCurve();

            alignStarts = DateTime.Now;
        }

        public virtual void Stop()
        {
            cts.Cancel();
        }

        /// <summary>
        /// Pause the feedback instruments due to the software reads the instruments continuously, the communication port
        /// is occupied, so the background reading loop should be halted while alignment process reading the instruments
        /// </summary>
        public virtual void PauseInstruments()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Release the instruments
        /// <see cref="PauseInstruments"/>
        /// </summary>
        public virtual void ResumeInstruments()
        {
            throw new NotImplementedException();
        }

    }
}
