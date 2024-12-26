namespace win_installer;
using System;
using System.Drawing;
using System.Windows.Forms;
using common;

/// <summary>
/// Represents the splash screen form for the Windows installer application.
/// The SplashForm serves as an introductory interface that displays an embedded logo,
/// a web browser control, and a continue button, which allows the user to proceed
/// to the next step of the application installation process.
/// </summary>
public sealed class SplashForm : Form
{
    // ===================== Beginning of configuration  
    private const           string WindowTitle = "Installation - welcome!";
    private static readonly Size   WindowSize  = new(600, 400);
    private const           string WindowImage = "win_installer.resources.click-and-crypt-square.jpg";
    private const string WindowText = """
                                       <h1>Click&amp;Crypt installation</h1>
                                       Please press &quot;Continue&quot;
                                      """;
    // ===================== End of configuration  
    
    private readonly TableLayoutPanel                         _grid;
    private readonly PictureBox                               _logo;
    private readonly Microsoft.Web.WebView2.WinForms.WebView2 _webBrowser;
    private readonly Button                                   _btnContinue;
    
    public SplashForm() {
        Text    = WindowTitle;
        Size    = WindowSize;
        Padding = new Padding(20);
        _logo = new PictureBox { Dock     = DockStyle.Fill,  
                                 SizeMode = PictureBoxSizeMode.Zoom,
                                 Size     = new Size(200, 200),
                                 Image    = EmbeddedImageLoader.LoadEmbeddedImage(WindowImage)
                               };
        _webBrowser  = new Microsoft.Web.WebView2.WinForms.WebView2 { Dock = DockStyle.Fill };
        _btnContinue = new Button { Text = "Continue", 
                                    Dock = DockStyle.Top, Height = 40 
                                  };
        _grid = new TableLayoutPanel { Dock        = DockStyle.Fill,
                                       ColumnCount = 2,
                                       RowCount    = 3,
                                     };
        _grid.Controls.Add(_logo, 0, 0);
        _grid.Controls.Add(_webBrowser, 1, 0);
        _grid.Controls.Add(new Spacer(1, 30) , 0, 1);
        _grid.Controls.Add(_btnContinue, 0, 2);
        _grid.SetColumnSpan(_btnContinue, 2);
        Controls.Add(_grid);
        _addCallbacks();
    }

    /// <summary>
    /// Configures event handlers for UI components in the SplashForm.
    /// Subscribes the continue button's Click event to its respective handler,
    /// initializes the WebView2 component, and assigns its initialization
    /// completion event to the appropriate callback.
    /// </summary>
    private void _addCallbacks() {
        FormClosing                                     += _onFormClosing;
        _btnContinue.Click                              += _btnContinue_Click;
        _webBrowser.CoreWebView2InitializationCompleted += _webView_CoreWebView2InitializationCompleted;
        _webBrowser.EnsureCoreWebView2Async();
    }

    /// <summary>
    /// Handles the Click event for the "Continue" button in the splash screen.
    /// Initiates the transition to the LicenseForm by creating an instance of
    /// LicenseForm, displaying it, and hiding the current splash form.
    /// </summary>
    /// <param name="sender">The source of the event. Typically, the "Continue" button.</param>
    /// <param name="e">Event data associated with the Click event.</param>
    private void _btnContinue_Click(object? sender, EventArgs e) {
        var licenseForm = new LicenseForm();
        licenseForm.Show();
        this.Hide();
    }

    /// <summary>
    /// Handles the CoreWebView2InitializationCompleted event for the WebView2 control.
    /// This method is invoked when the WebView2 component completes its initialization sequence.
    /// It preloads an HTML content into the initialized WebView2 control to display welcome information.
    /// </summary>
    /// <param name="sender">The source of the event, typically the WebView2 control.</param>
    /// <param name="e">Event data associated with the CoreWebView2InitializationCompleted event.</param>
    private void _webView_CoreWebView2InitializationCompleted(object? sender, EventArgs e) {
        if (_webBrowser.CoreWebView2 != null)
        {
            var hexColor = WinFormsUtilities.ColorToHex(this.BackColor);
            var htmlContent = $@"
                                 <html>
                                     <head>
                                         <style>
                                             body {{
                                                 font-family: Arial, sans-serif;
                                                 background-color: {hexColor};
                                                 text-align: center;
                                                 padding: 20px;
                                             }}
                                         </style>
                                     </head>
                                     <body>{WindowText}</body>
                                 </html>
                                 ";
            _webBrowser.CoreWebView2.NavigateToString(htmlContent);
        }
    }
    
    private static void _onFormClosing(object? sender, FormClosingEventArgs e) {
        Application.Exit();
    }
}