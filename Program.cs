#:sdk Microsoft.NET.Sdk
#:property OutputType=WinExe
#:property TargetFramework=net10.0-windows
#:property PublishAot=False
#:property UseWPF=False
#:property UseWindowsForms=True

// https://github.com/dotnet/winforms/issues/5071#issuecomment-908789632
Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

Application.OleRequired();
Application.EnableVisualStyles();
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
Application.SetCompatibleTextRenderingDefault(false);

using var form = new Form()
{
    Text = "Text Reader",
};

var textBox = new TextBox
{
    Multiline = true,
    Dock = DockStyle.Fill,
    ReadOnly = true,
    ScrollBars = ScrollBars.Both,
    WordWrap = false,
    ShortcutsEnabled = false,
    ContextMenuStrip = new ContextMenuStrip()
    {
        Items =
        {
            new ToolStripMenuItem("Copy", null, (s, e) =>
            {
                var textBox = form.Controls.OfType<TextBox>().FirstOrDefault();
                if (!string.IsNullOrEmpty(textBox?.SelectedText))
                {
                    Clipboard.SetText(textBox.SelectedText);
                }
            }, Keys.Control | Keys.C),
            new ToolStripMenuItem("Select All", null, (s, e) =>
            {
                var textBox = form.Controls.OfType<TextBox>().FirstOrDefault();
                textBox?.SelectAll();
            }, Keys.Control | Keys.A),
        }
    }
};
form.Controls.Add(textBox);
form.Controls.Add(new MenuStrip
{
    Dock = DockStyle.Top,
    Items =
    {
        new ToolStripMenuItem("File")
        {
            DropDownItems =
            {
                new ToolStripMenuItem("Open", null, (s, e) =>
                {
                    using var ofd = new OpenFileDialog
                    {
                        Title = "Open Text File",
                        Filter = "Text Files|*.txt|All Files|*.*",
                        RestoreDirectory = true,
                        ShowReadOnly = true,
                        CheckFileExists = true,
                    };

                    if (ofd.ShowDialog(form) == DialogResult.OK)
                    {
                        textBox.Text = File.ReadAllText(ofd.FileName);
                        form.Text = $"Text Reader - {Path.GetFileName(ofd.FileName)}";
                    }
                },Keys.Control | Keys.O),
                new ToolStripSeparator(),
                new ToolStripMenuItem("Exit", null, (s, e) => form.Close())
            }
        },
        new ToolStripMenuItem("Edit")
        {
            DropDownItems =
            {
                new ToolStripMenuItem("Copy", null, (s, e) =>
                {
                    if (!string.IsNullOrEmpty(textBox.SelectedText))
                    {
                        Clipboard.SetText(textBox.SelectedText);
                    }
                }, Keys.Control | Keys.C),
                new ToolStripMenuItem("Select All", null, (s, e) =>
                {
                    textBox.SelectAll();
                }, Keys.Control | Keys.A),
            }
        },
        new ToolStripMenuItem("Help")
        {
            DropDownItems =
            {
                new ToolStripMenuItem("About", null, (s, e) =>
                {
                    MessageBox.Show(form, "Simple Text Reader\nUsing Windows Forms in .NET 10", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
                })
            }
        }
    }
});


using var appContext = new ApplicationContext(form);
Application.Run(appContext);
