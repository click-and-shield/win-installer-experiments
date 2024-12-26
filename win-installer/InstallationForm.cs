namespace win_installer;
using System;
using System.Windows.Forms;
using common;


public sealed class InstallationForm : Form
{
    // ===================== Beginning of configuration  
    private const           string WindowTitle = "Installation";
    private const           string WindowText  = "Installation in progress... Please wait...";
    private static readonly Size   WindowSize  = new(600, 400);
    private const           string OperationSuccessImage = "win_installer.resources.thumb-up.png";
    // ===================== End of configuration  
    
    private readonly TableLayoutPanel                         _grid;
    private readonly ProgressBar                              _progressBar;
    private readonly PictureBox                               _operationSuccessImage;
    private readonly Button                                   _btnClose;
    private readonly Label                                    _labelText;
    private readonly Timer                                    _timer;

    public InstallationForm()  {
        Text    = WindowTitle;
        Size    = WindowSize;
        Padding = new Padding(20);
        
        _timer = new Timer { Interval = 3000 /* 3s */ };
        
        _labelText = new Label { Text        = WindowText, 
                                 Dock        = DockStyle.Top,
                                 Font        = new Font("Arial", 12, FontStyle.Bold),
                                 TextAlign   = ContentAlignment.MiddleCenter,
                               };
        _progressBar = new ProgressBar {
                                       Dock                  = DockStyle.Top,
                                       Height                = 30,
                                       Style                 = ProgressBarStyle.Marquee,
                                       MarqueeAnimationSpeed = 50
                                       };
        _operationSuccessImage = new PictureBox { Dock      = DockStyle.Fill,  
                                                  SizeMode  = PictureBoxSizeMode.Zoom,
                                                  Size      = new Size(200, 200),
                                                  Image     = EmbeddedImageLoader.LoadEmbeddedImage(OperationSuccessImage),
                                                  Visible   = false,
                                                  BackColor = Color.Transparent,
                                                };

        _btnClose = new Button { Text = "Close", 
                                 Dock = DockStyle.Top,
                                 Height = 40,
                                 Visible = false
                                };
        _grid = new TableLayoutPanel { Dock        = DockStyle.Fill,
                                       ColumnCount = 1,
                                       RowCount    = 7,
                                     };
        
        _grid.Controls.Add(_labelText, 0, 0);
        _grid.Controls.Add(new Spacer(1, 10) , 0, 1);
        _grid.Controls.Add(_operationSuccessImage, 0, 2);
        _grid.Controls.Add(new Spacer(1, 10) , 0, 3);
        _grid.Controls.Add(_progressBar, 0, 4);
        _grid.Controls.Add(new Spacer(1, 10) , 0, 5);
        _grid.Controls.Add(_btnClose, 0, 6);
        
        Controls.Add(_grid);
        _addCallbacks();
        _startInstallation();
    }

    private void _startInstallation() {
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void _addCallbacks() {
        FormClosing += _onFormClosing;
        _btnClose.Click                                 += _btnClose_Click;
    }

    private void Timer_Tick(object? sender, EventArgs e) {
        _showStatusSuccess();
    }
    
    
    private void _showStatusSuccess()
    {
        _progressBar.Visible           = false;
        _btnClose.Visible              = true;
        _operationSuccessImage.Visible = true;
        _labelText.Text                = "Installation completed successfully";
    }
    
    private static void _btnClose_Click(object? sender, EventArgs e) {
        Application.Exit();
    }

    private static void _onFormClosing(object? sender, FormClosingEventArgs e) {
        Application.Exit();
    }
}
