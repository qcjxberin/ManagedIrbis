﻿/* CheckBoxGroupTest.cs -- 
 * Ars Magna project, http://arsmagna.ru
 * -------------------------------------------------------
 * Status: poor
 */

#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AM;
using AM.Windows.Forms;

using CodeJam;

using IrbisUI;

using JetBrains.Annotations;

using ManagedIrbis;

using MoonSharp.Interpreter;

using Newtonsoft.Json;

#endregion

namespace UITests
{
    public sealed class CheckBoxGroupTest
        : IUITest
    {
        #region IUITest members

        public void Run
            (
                IWin32Window ownerWindow
            )
        {
            using (Form form = new Form())
            {
                form.Size = new Size(800, 600);

                CheckBoxGroup group = new CheckBoxGroup
                {
                    Location = new Point(10, 10),
                    Size = new Size(200, 200),
                    Text = "Group of CheckBoxes",
                    Lines = new []
                    {
                        "One", "Two", "Three",
                        "Four", "Five", "Six"
                    }
                };
                form.Controls.Add(group);

                TextBox textBox = new TextBox
                {
                    Location = new Point(220, 10),
                    Width = 100
                };
                form.Controls.Add(textBox);

                group.CurrentChanged += (sender, current) =>
                {
                    textBox.Text = group.Current.ToString("X8");
                };

                form.ShowDialog(ownerWindow);
            }
        }

        #endregion
    }
}
