using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using Irixi_Aligner_Common.UserScript;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Irixi_Aligner_Common.View.ListBasedUserScript
{
    /// <summary>
    /// Interaction logic for ListBasedUserScriptEditor.xaml
    /// </summary>
    public partial class ListBasedUserScriptEditor : UserControl
    {
        public ListBasedUserScriptEditor()
        {
            InitializeComponent();
        }

        void CustomRowAppearance(object sender, CustomRowAppearanceEventArgs e)
        {
            if (e.RowSelectionState != SelectionState.None)
            {
                object result = e.ConditionalValue;
                if (e.Property == TextBlock.ForegroundProperty || e.Property == TextBlock.BackgroundProperty)
                {
                    SolidColorBrush original = e.OriginalValue as SolidColorBrush;
                    SolidColorBrush conditional = e.ConditionalValue as SolidColorBrush;
                    if (conditional != null && (original == null || original.Color != conditional.Color))
                        result = ShadeBrush(conditional);
                }
                e.Result = result;
                e.Handled = true;
            }
        }

        SolidColorBrush ShadeBrush(SolidColorBrush brush)
        {
            Color originalColor = brush.Color;
            float coefficient = 0.75f;
            byte a = originalColor.A;
            if (!gridScriptAdded.IsKeyboardFocusWithin)
                a = (byte)(originalColor.A / 2);
            byte r = (byte)(originalColor.R * coefficient);
            byte g = (byte)(originalColor.G * coefficient);
            byte b = (byte)(originalColor.B * coefficient);
            return new SolidColorBrush(Color.FromArgb(a, r, g, b));
        }

        private void OnDragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            if (e.IsFromOutside && typeof(IUserScript).IsAssignableFrom(e.GetRecordType()))
                e.Effects = DragDropEffects.Move;

            e.Handled = true;
        }

        private void ListBoxEdit_StartRecordDrag(object sender, StartRecordDragEventArgs e)
        {
            // Create the command instance according to the type in the dragged source.

            List<IUserScript> newData = new List<IUserScript>();

            foreach (dynamic item in e.Records)
                newData.Add((IUserScript)Activator.CreateInstance((Type)item.ObjectType));

            e.Data.SetData(new RecordDragDropData(newData.ToArray()));

        }

        private void CompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e)
        {
            // do not detete items from the source list.
            e.Handled = true;
        }

        private void TbvCommandSource_DragRecordOver(object sender, DragRecordOverEventArgs e)
        {
            if (e.IsFromOutside == false)
                e.Effects = DragDropEffects.None;
            e.Handled = true;
        }
    }
}
