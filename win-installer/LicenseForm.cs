namespace win_installer;
using System;
using System.Windows.Forms;

/// <summary>
/// Represents a form that displays the license agreement for the application during the installation process.
/// LicenseForm provides an interactive interface where a user can read the license terms, confirm acceptance,
/// and proceed further in the installation process.
/// </summary>
public sealed class LicenseForm : Form
{
    // ===================== Beginning of configuration  
    private const           string WindowTitle = "Installation - user licence!";
    private static readonly Size   WindowSize  = new(600, 400);
    private const           string AgreementText = "I agree to the terms of the license";
    // ===================== End of configuration  
    
    private readonly CheckBox    _chkAcceptLicense;
    private readonly Button      _btnContinue;
    private readonly RichTextBox _rtbLicense;

    public LicenseForm() {
        Text    = WindowTitle;
        Size    = WindowSize;
        Padding = new Padding(20);
        
        // RichTextBox used to elegantly display the license terms
        _rtbLicense = new RichTextBox
                      { Rtf        = License.GetLicence(),
                        Dock       = DockStyle.Top,
                        Height     = 250,
                        ScrollBars = RichTextBoxScrollBars.Vertical,
                        WordWrap   = false,
                        Multiline  = true,
                        Visible    = true,
                        ReadOnly   = true,
                        BackColor  = Color.White, };
        _chkAcceptLicense = new CheckBox { Text   = AgreementText, 
                                           Dock   = DockStyle.Top,
                                           Height = 30 };
        _btnContinue = new Button { Text  = "Continue", 
                                    Dock = DockStyle.Top, 
                                    Height  = 40, 
                                    Visible = false };
        Controls.Add(_btnContinue);
        Controls.Add(_chkAcceptLicense);
        Controls.Add(_rtbLicense);
        _addCallbacks();
    }

    /// <summary>
    /// Registers the event handlers for the interactive controls within the license form.
    /// Binds the appropriate event callbacks to the CheckBox and Button controls
    /// to ensure the user interface responds correctly to user input.
    /// </summary>
    private void _addCallbacks() {
        FormClosing                      += _onFormClosing;
        _chkAcceptLicense.CheckedChanged += _chkAcceptLicense_CheckedChanged;
        _btnContinue.Click               += _btnContinue_Click;

    }

    /// <summary>
    /// Handles the CheckedChanged event of the license acceptance CheckBox.
    /// Toggles the visibility of the "Continue" button depending on whether the user has checked
    /// the agreement CheckBox to accept the license terms.
    /// </summary>
    /// <param name="sender">The source of the event, typically the CheckBox control.</param>
    /// <param name="e">An event data object containing event-specific information.</param>
    private void _chkAcceptLicense_CheckedChanged(object? sender, EventArgs e) {
        _btnContinue.Visible = _chkAcceptLicense.Checked;
    }

    /// <summary>
    /// Handles the click event of the "Continue" button on the LicenseForm,
    /// which initiates the transition to the InstallationForm.
    /// When invoked, this method hides the current license agreement form
    /// and displays the installation form, allowing the user to proceed
    /// with the installation process.
    /// </summary>
    /// <param name="sender">The source object triggering the event, typically the "Continue" button.</param>
    /// <param name="e">An EventArgs object containing any event-specific details.</param>
    private void _btnContinue_Click(object? sender, EventArgs e) {
        InstallationForm installationForm = new InstallationForm();
        installationForm.Show();
        this.Hide();
    }
    
    private static void _onFormClosing(object? sender, FormClosingEventArgs e) {
        Application.Exit();
    }
}
