using System;
using System.Drawing;
using System.Windows.Forms;

namespace Higurashi_Rescripter
{
    public partial class FormMain : CustomUI.Forms.CustomForm
    {

        //
        // Private variables
        //

        private Config config;
        private ErrorHandler error;
        private ScriptEngine engine;

        private int logoIndex;


        //
        // Constructor
        //

        public FormMain()
        {
            InitializeComponent();

            InitializeApp();

            RandomizeLogo();
        }


        //
        // Methods
        //

        // Load settings and initialize script engine
        private void InitializeApp()
        {
            error = new ErrorHandler();

            try
            {
                config = new Config(error);
                engine = new ScriptEngine(config, error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Higurashi Rescripter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                error.SaveLog();
                Application.Exit();
            }

            logoIndex = -1;
        }

        // Set a random logo
        private void RandomizeLogo()
        {
            //
            // Always set a different logo than the current one
            //

            int tempLogoIndex;

            do
            {
                tempLogoIndex = new Random().Next(1, 10);
            } while (logoIndex == tempLogoIndex);

            logoIndex = tempLogoIndex;

            // Set the logo
            pictureBoxLogo.Image = (Image)Properties.Resources.ResourceManager.GetObject($"logo_full_{logoIndex}");
        }

        // Set random logo on click event
        private void pictureBoxLogo_Click(object sender, EventArgs e)
        {
            if (((MouseEventArgs)e).Button == MouseButtons.Left)
            {
                RandomizeLogo();
            }
        }

        // Generate sheets
        private void buttonGenerateSheet_Click(object sender, EventArgs e)
        {
            try
            {
                engine.GenerateSheets();

                MessageBox.Show("Operazione completata!", "Higurashi Rescripter", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Higurashi Rescripter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                error.SaveLog();
            }
        }

        // Generate scripts
        private void buttonGenerateScript_Click(object sender, EventArgs e)
        {
            try
            {
                engine.GenerateScripts();

                MessageBox.Show("Operazione completata!", "Higurashi Rescripter", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Higurashi Rescripter", MessageBoxButtons.OK, MessageBoxIcon.Error);
                error.SaveLog();
            }
        }

    }
}
