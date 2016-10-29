using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System.Windows.Forms;
using MetroFramework.Forms;
using MetroFramework.Controls;
using System.Drawing;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;


namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    public class MyDashboard : Robot
    {
        // cBot Parameters
        [Parameter("Default Symbols", DefaultValue = "EURUSD GBPUSD EURJPY USDJPY AUDUSD USDCHF GBPJPY USDCAD EURGBP EURCHF AUDJPY NZDUSD CHFJPY EURAUD CADJPY GBPAUD EURCAD AUDCAD GBPCAD AUDNZD NZDJPY AUDCHF GBPNZD EURNZD CADCHF NZDCAD NZDCHF GBPCHF XAUUSD XAGUSD")]
        public string defaultSymbolsString { get; set; }


        // Controls
        private MetroForm DashboardForm;
        private MetroForm SymbolsForm;
        private MetroTabControl Tabs;
        private MetroComboBox comboBoxCorrelationSymbols;
        private MetroProgressBar SymbolsProgressBar;
        private MetroLabel LabelWait;
        private MetroGrid currenciesGrid;
        private MetroGrid pairsGrid;
        private MetroGrid volatilityGrid;
        private MetroGrid correlationGrid;
        private MetroGrid trendGrid;
        private MetroGrid symbolsGrid;
        private MetroTextBox swSMAPeriods;
        private MetroTextBox correlationPeriods;
        private MetroTextBox sdPeriods;
        private MetroTextBox atrPeriods;
        private MetroTextBox abrPeriods;
        private MetroTextBox chaikinPeriods;
        private MetroTextBox chaikinRate;
        private MetroComboBox comboBoxSeriesTF;
        private MetroComboBox comboBoxMaType;
        private MetroTextBox maPeriods;
        private MetroTextBox rsiPeriods;
        private MetroTextBox rsiSeperator;
        private MetroTextBox macdLongCycle;
        private MetroTextBox macdShortCycle;
        private MetroTextBox macdSignalPeriods;
        private MetroTextBox updateMinutes;
        private MetroTextBox symbolTextBox;
        private MetroButton updateButton;
        private MetroButton symbolsButton;
        private MetroButton sortSettingsButton;
        private MetroButton deleteSymbolsButton;

        // Sort settings form controls
        private MetroLabel correlationGridLabel;
        private MetroLabel trendGridLabel;
        private MetroLabel volatilityGridLabel;
        private MetroComboBox correlationColumnsComboBox;
        private MetroComboBox crrelationSortDirectionComboBox;
        private MetroComboBox volatilityColumnComboBox;
        private MetroComboBox volatilitySortDirectionComboBox;
        private MetroComboBox trendColumnsComboBox;
        private MetroComboBox trendSortDirectionComboBox;
        private MetroButton sortSettingOkButton;
        private MetroForm sortSettingsForm;


        // Collections
        private List<string> allSymbols = new List<string>();
        private List<string> currencies = new List<string>();
        private Dictionary<string, List<string>> currenciesWithSymbols = new Dictionary<string, List<string>>();
        private Dictionary<string, MarketSeries> SymbolsSeries = new Dictionary<string, MarketSeries>();
        private Dictionary<string, TimeFrame> SeriesTimeFrames = new Dictionary<string, TimeFrame>();

        private System.Timers.Timer checkFormTimer;

        private int correlationGridSortColumnIndex = 0;
        private ListSortDirection correlationGridSortDirection = ListSortDirection.Ascending;
        private int volatilityGridSortColumnIndex = 0;
        private ListSortDirection volatilityGridSortDirection = ListSortDirection.Ascending;
        private int trendGridSortColumnIndex = 0;
        private ListSortDirection trendGridSortDirection = ListSortDirection.Ascending;

        protected override void OnStart()
        {
            string[] splittedSymbolsString = defaultSymbolsString.Split(new char[] 
            {
                ' '
            });
            allSymbols.AddRange(splittedSymbolsString);

            FillCurrencies();



            SeriesTimeFrames.Add("1 Minute", TimeFrame.Minute);
            SeriesTimeFrames.Add("5 Minute", TimeFrame.Minute5);
            SeriesTimeFrames.Add("15 Minute", TimeFrame.Minute15);
            SeriesTimeFrames.Add("30 Minute", TimeFrame.Minute30);
            SeriesTimeFrames.Add("1 Hour", TimeFrame.Hour);
            SeriesTimeFrames.Add("4 Hour", TimeFrame.Hour4);
            SeriesTimeFrames.Add("Daily", TimeFrame.Daily);

            Task showDashboardForm = Task.Factory.StartNew(() => { DashboardFormInitializer(); });

            checkFormTimer = new System.Timers.Timer(1000);
            checkFormTimer.Elapsed += new ElapsedEventHandler(CheckForm);
            checkFormTimer.Start();
        }


        // Forms Initializers

        private void DashboardFormInitializer()
        {
            // Initializing Form
            DashboardForm = new MetroForm();
            DashboardForm.Name = "DashboardForm";
            DashboardForm.Text = "afhacker Dashboard";
            DashboardForm.Size = new Size(775, 533);
            DashboardForm.MaximizeBox = false;
            DashboardForm.StartPosition = FormStartPosition.CenterScreen;
            DashboardForm.Resizable = false;
            DashboardForm.ShowIcon = false;
            DashboardForm.Theme = MetroFramework.MetroThemeStyle.Dark;
            DashboardForm.Style = MetroFramework.MetroColorStyle.Silver;
            DashboardForm.Load += new EventHandler(DashboardFormLoad);
            DashboardForm.FormClosed += new FormClosedEventHandler(DashboardFormClosed);


            // Initializing Progress Bar
            SymbolsProgressBar = new MetroProgressBar();
            SymbolsProgressBar.Name = "ProgressBar";
            SymbolsProgressBar.Maximum = allSymbols.Count;
            SymbolsProgressBar.Theme = MetroFramework.MetroThemeStyle.Dark;
            SymbolsProgressBar.Location = new Point(37, 500);
            SymbolsProgressBar.Size = new Size(305, 23);

            // Initializing TabControl
            Tabs = new MetroTabControl();
            Tabs.Name = "Tabs";
            Tabs.Size = new Size(711, 438);
            Tabs.Location = new Point(33, 60);
            Tabs.ItemSize = new Size(96, 34);
            Tabs.Theme = MetroFramework.MetroThemeStyle.Dark;


            // Initializing TabPages
            TabPage startTab = new MetroTabPage();
            startTab.Name = "startTab";
            startTab.Text = "Start";
            startTab.Size = new Size(703, 396);

            TabPage strongWeakTab = new MetroTabPage();
            strongWeakTab.Name = "StrongWeakTab";
            strongWeakTab.Text = "Strong / Weak";
            strongWeakTab.Size = new Size(703, 396);

            TabPage correlationTab = new MetroTabPage();
            correlationTab.Name = "CorrelationTab";
            correlationTab.Text = "Correlation";
            correlationTab.Size = new Size(703, 396);

            TabPage volatilityTab = new MetroTabPage();
            volatilityTab.Name = "volatilityTab";
            volatilityTab.Text = "Volatility";
            volatilityTab.Size = new Size(703, 396);

            TabPage trendTab = new MetroTabPage();
            trendTab.Name = "trendTab";
            trendTab.Text = "Trend";
            trendTab.Size = new Size(703, 396);

            TabPage aboutTab = new MetroTabPage();
            aboutTab.Name = "aboutTab";
            aboutTab.Text = "About";
            aboutTab.Size = new Size(703, 396);


            // Initializing Labels

            LabelWait = new MetroLabel();
            LabelWait.Name = "labelWait";
            LabelWait.Text = "Please Wait...";
            LabelWait.Theme = MetroFramework.MetroThemeStyle.Dark;
            LabelWait.Size = new Size(90, 19);
            LabelWait.Location = new Point(242, 29);

            // Strong / Weak Tab Labels

            MetroLabel labelCurrencies = new MetroLabel();
            labelCurrencies.Name = "labelCurrencies";
            labelCurrencies.Text = "Currencies";
            labelCurrencies.Size = new Size(79, 19);
            labelCurrencies.Location = new Point(142, 35);
            labelCurrencies.FontWeight = MetroFramework.MetroLabelWeight.Bold;

            MetroLabel labelPairs = new MetroLabel();
            labelPairs.Name = "labelPairs";
            labelPairs.Text = "Pairs";
            labelPairs.Size = new Size(42, 19);
            labelPairs.Location = new Point(487, 35);
            labelPairs.FontWeight = MetroFramework.MetroLabelWeight.Bold;

            // Correlation Tab Labels

            MetroLabel labelCorrelationSelectSymbol = new MetroLabel();
            labelCorrelationSelectSymbol.Name = "labelCorrelation";
            labelCorrelationSelectSymbol.Text = "Select Symbol:";
            labelCorrelationSelectSymbol.Size = new Size(116, 19);
            labelCorrelationSelectSymbol.Location = new Point(30, 32);
            labelCorrelationSelectSymbol.FontWeight = MetroFramework.MetroLabelWeight.Bold;

            MetroLabel labelCorrelationDescription = new MetroLabel();
            labelCorrelationDescription.Name = "labelCorrelationDescription";
            labelCorrelationDescription.Text = "For making it more simple the decimal numbers\n" + "are changed to percentage.\n" + "\n" + "What is 'Correlation'?\n" + "Correlation, in the finance and investment\n" + "industries, is a statistic that measures the\n" + "degree to which two securities move in\n" + "relation to each other. Correlations are\n" + "used in advanced portfolio management.\n" + "Correlation is computed into what is\n" + "known as the correlation coefficient, which\n" + "has value that must fall between -1 (-100%)\n" + "and 1 (100%).\n";
            labelCorrelationDescription.Size = new Size(296, 247);
            labelCorrelationDescription.Location = new Point(359, 79);

            // About Tab Labels

            MetroLabel labelAboutDeveloper = new MetroLabel();
            labelAboutDeveloper.Name = "labelAboutDeveloper";
            labelAboutDeveloper.Text = "Developed By: afhacker(Ahmad Noman Musleh)";
            labelAboutDeveloper.Size = new Size(294, 23);
            labelAboutDeveloper.Location = new Point(400, 18);

            MetroLabel labelAboutVersion = new MetroLabel();
            labelAboutVersion.Name = "labelAboutVersion";
            labelAboutVersion.Text = string.Format("Version: {0}", Application.ProductVersion);
            labelAboutVersion.Size = new Size(290, 23);
            labelAboutVersion.Location = new Point(400, 56);

            MetroLabel labelAboutContact = new MetroLabel();
            labelAboutContact.Name = "labelAboutContact";
            labelAboutContact.Text = "Contact Information";
            labelAboutContact.Size = new Size(143, 19);
            labelAboutContact.Location = new Point(400, 91);
            labelAboutContact.FontWeight = MetroFramework.MetroLabelWeight.Bold;

            MetroLabel labelAboutEmail = new MetroLabel();
            labelAboutEmail.Name = "labelAboutEmail";
            labelAboutEmail.Text = "Email: ";
            labelAboutEmail.Size = new Size(60, 23);
            labelAboutEmail.Location = new Point(400, 126);

            MetroTextBox txtBoxAboutEmail = new MetroTextBox();
            txtBoxAboutEmail.Name = "txtBoxAboutEmail";
            txtBoxAboutEmail.Text = "afhackermubasher@gmail.com";
            txtBoxAboutEmail.ReadOnly = true;
            txtBoxAboutEmail.Size = new Size(230, 23);
            txtBoxAboutEmail.Location = new Point(460, 126);

            MetroLabel labelAboutSkype = new MetroLabel();
            labelAboutSkype.Name = "labelAboutSkype";
            labelAboutSkype.Text = "Skype: ";
            labelAboutSkype.Size = new Size(60, 23);
            labelAboutSkype.Location = new Point(400, 164);

            MetroTextBox txtBoxAboutSkype = new MetroTextBox();
            txtBoxAboutSkype.Name = "txtBoxAboutSkype";
            txtBoxAboutSkype.Text = "nomanmubasher";
            txtBoxAboutSkype.ReadOnly = true;
            txtBoxAboutSkype.Size = new Size(230, 23);
            txtBoxAboutSkype.Location = new Point(460, 164);

            MetroLabel labelAboutDonation = new MetroLabel();
            labelAboutDonation.Name = "labelAboutDonation";
            labelAboutDonation.Text = "Donation";
            labelAboutDonation.Size = new Size(70, 19);
            labelAboutDonation.Location = new Point(400, 214);
            labelAboutDonation.FontWeight = MetroFramework.MetroLabelWeight.Bold;

            MetroLabel labelAboutBitcoin = new MetroLabel();
            labelAboutBitcoin.Name = "labelAboutBitcoin";
            labelAboutBitcoin.Text = "BitCoin: ";
            labelAboutBitcoin.TextAlign = ContentAlignment.TopLeft;
            labelAboutBitcoin.Size = new Size(60, 23);
            labelAboutBitcoin.Location = new Point(400, 253);

            MetroTextBox txtBoxAboutBitcoin = new MetroTextBox();
            txtBoxAboutBitcoin.Name = "txtBoxAboutBitcoin";
            txtBoxAboutBitcoin.Text = "1Me2TQ6Rgr8EevZNFKqvYegkbtY8C2cP5i";
            txtBoxAboutBitcoin.ReadOnly = true;
            txtBoxAboutBitcoin.Size = new Size(230, 23);
            txtBoxAboutBitcoin.Location = new Point(460, 253);

            MetroLabel labelAboutPaypal = new MetroLabel();
            labelAboutPaypal.Name = "labelAboutPaypal";
            labelAboutPaypal.Text = "Paypal: ";
            labelAboutPaypal.Size = new Size(60, 23);
            labelAboutPaypal.Location = new Point(400, 291);

            MetroTextBox txtBoxAboutPaypal = new MetroTextBox();
            txtBoxAboutPaypal.Name = "txtBoxAboutPaypal";
            txtBoxAboutPaypal.Text = "sear.wazeen@yahoo.com";
            txtBoxAboutPaypal.ReadOnly = true;
            txtBoxAboutPaypal.Size = new Size(230, 23);
            txtBoxAboutPaypal.Location = new Point(460, 291);

            // Start Tab Labels
            MetroLabel labelStartWelcome = new MetroLabel();
            labelStartWelcome.Name = "labelStartWelcome";
            labelStartWelcome.Text = "Welcom To My Dashboard";
            labelStartWelcome.Size = new Size(186, 19);
            labelStartWelcome.Location = new Point(254, 9);
            labelStartWelcome.FontWeight = MetroFramework.MetroLabelWeight.Bold;

            MetroLabel labelStartSeriesTF = new MetroLabel();
            labelStartSeriesTF.Name = "labelStartSeriesTF";
            labelStartSeriesTF.Text = "Series Time Frame :";
            labelStartSeriesTF.Size = new Size(179, 19);
            labelStartSeriesTF.Location = new Point(34, 52);

            MetroLabel labelStartCorrelationPeriods = new MetroLabel();
            labelStartCorrelationPeriods.Name = "labelStartCorrelationPeriods";
            labelStartCorrelationPeriods.Text = "Correlation Periods : ";
            labelStartCorrelationPeriods.Size = new Size(133, 19);
            labelStartCorrelationPeriods.Location = new Point(34, 88);

            MetroLabel labelStartSDPeriods = new MetroLabel();
            labelStartSDPeriods.Name = "labelStartSDPeriods";
            labelStartSDPeriods.Text = "Standard Deviation Periods :";
            labelStartSDPeriods.Size = new Size(174, 19);
            labelStartSDPeriods.Location = new Point(34, 124);

            MetroLabel labelStartATRPeriods = new MetroLabel();
            labelStartATRPeriods.Name = "labelStartATRPeriods";
            labelStartATRPeriods.Text = "ATR Periods :";
            labelStartATRPeriods.Size = new Size(85, 19);
            labelStartATRPeriods.Location = new Point(34, 160);

            MetroLabel labelStartABRPeriods = new MetroLabel();
            labelStartABRPeriods.Name = "labelStartABRPeriods";
            labelStartABRPeriods.Text = "ABR Periods :";
            labelStartABRPeriods.Size = new Size(88, 19);
            labelStartABRPeriods.Location = new Point(34, 196);

            MetroLabel labelStartChaikinVolatilityPeriods = new MetroLabel();
            labelStartChaikinVolatilityPeriods.Name = "labelStartChaikinVolatilityPeriods";
            labelStartChaikinVolatilityPeriods.Text = "Chaikin Volatility Periods :";
            labelStartChaikinVolatilityPeriods.Size = new Size(157, 19);
            labelStartChaikinVolatilityPeriods.Location = new Point(34, 232);

            MetroLabel labelStartChaikinRateChange = new MetroLabel();
            labelStartChaikinRateChange.Name = "labelStartChaikinRateChange";
            labelStartChaikinRateChange.Text = "Chaikin Rate Of Change :";
            labelStartChaikinRateChange.Size = new Size(156, 19);
            labelStartChaikinRateChange.Location = new Point(34, 268);

            MetroLabel labelStartUpdate = new MetroLabel();
            labelStartUpdate.Name = "labelStartUpdate";
            labelStartUpdate.Text = "Auto Update After(Minutes) :";
            labelStartUpdate.Size = new Size(185, 19);
            labelStartUpdate.Location = new Point(34, 304);

            MetroLabel labelStartMovingAverageType = new MetroLabel();
            labelStartMovingAverageType.Name = "labelStartMovingAverageType";
            labelStartMovingAverageType.Text = "Moving Average Type :";
            labelStartMovingAverageType.Size = new Size(144, 19);
            labelStartMovingAverageType.Location = new Point(334, 52);

            MetroLabel labelStartMovingAveragePeriods = new MetroLabel();
            labelStartMovingAveragePeriods.Name = "labelStartMovingAveragePeriods";
            labelStartMovingAveragePeriods.Text = "Moving Average Periods :";
            labelStartMovingAveragePeriods.Size = new Size(160, 19);
            labelStartMovingAveragePeriods.Location = new Point(334, 88);

            MetroLabel labelStartRSIPeriods = new MetroLabel();
            labelStartRSIPeriods.Name = "labelStartRSIPeriods";
            labelStartRSIPeriods.Text = "RSI Periods :";
            labelStartRSIPeriods.Size = new Size(81, 19);
            labelStartRSIPeriods.Location = new Point(334, 124);

            MetroLabel labelStartRSITrendSeperator = new MetroLabel();
            labelStartRSITrendSeperator.Name = "labelStartRSITrendSeperator";
            labelStartRSITrendSeperator.Text = "RSI Trend Seperator :";
            labelStartRSITrendSeperator.Size = new Size(132, 19);
            labelStartRSITrendSeperator.Location = new Point(334, 160);

            MetroLabel labelStartMACDLongCycle = new MetroLabel();
            labelStartMACDLongCycle.Name = "labelStartMACDLongCycle";
            labelStartMACDLongCycle.Text = "MACD Long Cycle :";
            labelStartMACDLongCycle.Size = new Size(123, 19);
            labelStartMACDLongCycle.Location = new Point(334, 196);

            MetroLabel labelStartMACDShortCycle = new MetroLabel();
            labelStartMACDShortCycle.Name = "labelStartMACDShortCycle";
            labelStartMACDShortCycle.Text = "MACD Short Cycle :";
            labelStartMACDShortCycle.Size = new Size(126, 19);
            labelStartMACDShortCycle.Location = new Point(334, 232);

            MetroLabel labelStartMACDSignalPeriods = new MetroLabel();
            labelStartMACDSignalPeriods.Name = "labelStartMACDSignalPeriods";
            labelStartMACDSignalPeriods.Text = "MACD Signal Periods :";
            labelStartMACDSignalPeriods.Size = new Size(141, 19);
            labelStartMACDSignalPeriods.Location = new Point(334, 268);

            MetroLabel labelStartSWSMAPeriods = new MetroLabel();
            labelStartSWSMAPeriods.Name = "labelStartSWSMAPeriods";
            labelStartSWSMAPeriods.Text = "Strong / Weak SMA Periods :";
            labelStartSWSMAPeriods.Size = new Size(179, 19);
            labelStartSWSMAPeriods.Location = new Point(334, 304);

            // Initializing Link Labels
            MetroLink investopediaLink = new MetroLink();
            investopediaLink.Name = "investopediaLink";
            investopediaLink.Text = "http://www.investopedia.com/terms/c/correlation.asp";
            investopediaLink.TextAlign = ContentAlignment.TopLeft;
            investopediaLink.Size = new Size(311, 23);
            investopediaLink.Location = new Point(359, 332);
            investopediaLink.Click += new EventHandler(OpenInvestopediaLink);


            MetroLink dailyFXLink = new MetroLink();
            dailyFXLink.Name = "dailyFXLink";
            dailyFXLink.Text = "More Detail";
            dailyFXLink.TextAlign = ContentAlignment.TopLeft;
            dailyFXLink.Size = new Size(311, 23);
            dailyFXLink.Location = new Point(315, 370);
            dailyFXLink.Click += new EventHandler(OpenDailyFXLink);
            dailyFXLink.FontWeight = MetroFramework.MetroLinkWeight.Bold;

            MetroLink ctdnLink = new MetroLink();
            ctdnLink.Name = "ctdnLink";
            ctdnLink.Text = "cTDN";
            ctdnLink.TextAlign = ContentAlignment.TopLeft;
            ctdnLink.Size = new Size(75, 23);
            ctdnLink.Location = new Point(501, 354);
            ctdnLink.Click += new EventHandler(OpencTDNLink);
            ctdnLink.FontWeight = MetroFramework.MetroLinkWeight.Bold;

            // Initializing PictureBox

            PictureBox picBox = new PictureBox();
            picBox.Name = "afhackerImage";
            picBox.ImageLocation = "http://ctdn.com/uploads/afhacker/avatar/62e9d20ba60c4ab34d8e87d78506c92f5303b1e5.gif";
            picBox.Size = new Size(358, 359);
            picBox.Location = new Point(19, 18);
            picBox.SizeMode = PictureBoxSizeMode.StretchImage;

            // Initializing TexBoxs

            correlationPeriods = new MetroTextBox();
            correlationPeriods.Name = "correlationPeriods";
            correlationPeriods.Text = "50";
            correlationPeriods.Size = new Size(100, 29);
            correlationPeriods.Location = new Point(219, 88);

            sdPeriods = new MetroTextBox();
            sdPeriods.Name = "sdPeriods";
            sdPeriods.Text = "14";
            sdPeriods.Size = new Size(100, 29);
            sdPeriods.Location = new Point(219, 124);

            atrPeriods = new MetroTextBox();
            atrPeriods.Name = "atrPeriods";
            atrPeriods.Text = "14";
            atrPeriods.Size = new Size(100, 29);
            atrPeriods.Location = new Point(219, 160);

            abrPeriods = new MetroTextBox();
            abrPeriods.Name = "abrPeriods";
            abrPeriods.Text = "10";
            abrPeriods.Size = new Size(100, 29);
            abrPeriods.Location = new Point(219, 196);

            chaikinPeriods = new MetroTextBox();
            chaikinPeriods.Name = "chaikinPeriods";
            chaikinPeriods.Text = "14";
            chaikinPeriods.Size = new Size(100, 29);
            chaikinPeriods.Location = new Point(219, 232);

            chaikinRate = new MetroTextBox();
            chaikinRate.Name = "chaikinRate";
            chaikinRate.Text = "10";
            chaikinRate.Size = new Size(100, 29);
            chaikinRate.Location = new Point(219, 268);

            updateMinutes = new MetroTextBox();
            updateMinutes.Name = "updateMinutes";
            updateMinutes.Text = "60";
            updateMinutes.Size = new Size(100, 29);
            updateMinutes.Location = new Point(219, 304);

            maPeriods = new MetroTextBox();
            maPeriods.Name = "maPeriods";
            maPeriods.Text = "20";
            maPeriods.Size = new Size(100, 29);
            maPeriods.Location = new Point(528, 88);

            rsiPeriods = new MetroTextBox();
            rsiPeriods.Name = "rsiPeriods";
            rsiPeriods.Text = "14";
            rsiPeriods.Size = new Size(100, 29);
            rsiPeriods.Location = new Point(528, 124);

            rsiSeperator = new MetroTextBox();
            rsiSeperator.Name = "rsiSeperator";
            rsiSeperator.Text = "50";
            rsiSeperator.Size = new Size(100, 29);
            rsiSeperator.Location = new Point(528, 160);

            macdLongCycle = new MetroTextBox();
            macdLongCycle.Name = "macdLongCycle";
            macdLongCycle.Text = "26";
            macdLongCycle.Size = new Size(100, 29);
            macdLongCycle.Location = new Point(528, 196);

            macdShortCycle = new MetroTextBox();
            macdShortCycle.Name = "macdShortCycle";
            macdShortCycle.Text = "12";
            macdShortCycle.Size = new Size(100, 29);
            macdShortCycle.Location = new Point(528, 232);

            macdSignalPeriods = new MetroTextBox();
            macdSignalPeriods.Name = "macdSignalPeriods";
            macdSignalPeriods.Text = "9";
            macdSignalPeriods.Size = new Size(100, 29);
            macdSignalPeriods.Location = new Point(528, 268);

            swSMAPeriods = new MetroTextBox();
            swSMAPeriods.Name = "swSMAPeriods";
            swSMAPeriods.Text = "200";
            swSMAPeriods.Size = new Size(100, 29);
            swSMAPeriods.Location = new Point(528, 304);


            // Initializing ComboBox

            comboBoxCorrelationSymbols = new MetroComboBox();
            comboBoxCorrelationSymbols.Name = "comboBoxCorrelationSymbols";
            comboBoxCorrelationSymbols.PromptText = "Symbols...";
            comboBoxCorrelationSymbols.Location = new Point(153, 32);
            comboBoxCorrelationSymbols.Size = new Size(121, 29);
            comboBoxCorrelationSymbols.SelectedIndexChanged += new EventHandler(comboBoxSymbolSelected);




            comboBoxSeriesTF = new MetroComboBox();
            comboBoxSeriesTF.Name = "comboBoxSeriesTF";
            comboBoxSeriesTF.Location = new Point(219, 52);
            comboBoxSeriesTF.Size = new Size(100, 29);

            foreach (string tfLabel in SeriesTimeFrames.Keys)
            {
                comboBoxSeriesTF.Items.Add(tfLabel);

            }

            comboBoxSeriesTF.SelectedIndex = 6;

            comboBoxMaType = new MetroComboBox();
            comboBoxMaType.Name = "comboBoxMaType";
            comboBoxMaType.Location = new Point(528, 52);
            comboBoxMaType.Size = new Size(100, 29);
            comboBoxMaType.Items.Add("Simple");
            comboBoxMaType.Items.Add("Exponential");
            comboBoxMaType.SelectedIndex = 0;

            // Initializing MetroButton


            symbolsButton = new MetroButton();
            symbolsButton.Name = "symbolsButton";
            symbolsButton.Text = "Symbols";
            symbolsButton.Size = new Size(75, 23);
            symbolsButton.Location = new Point(215, 360);
            symbolsButton.Click += new EventHandler(SymbolsClicked);

            updateButton = new MetroButton();
            updateButton.Name = "updateButton";
            updateButton.Text = "Update";
            updateButton.Size = new Size(75, 23);
            updateButton.Location = new Point(315, 360);
            updateButton.Click += new EventHandler(UpdateClicked);

            sortSettingsButton = new MetroButton();
            sortSettingsButton.Name = "sortSettingsButton";
            sortSettingsButton.Text = "Sort Settings";
            sortSettingsButton.Size = new Size(100, 23);
            sortSettingsButton.Location = new Point(415, 360);
            sortSettingsButton.Click += new EventHandler(SortSettingsClicked);

            // Initializing MetroGrid

            currenciesGrid = new MetroGrid();
            currenciesGrid.Name = "currenciesGrid";
            currenciesGrid.Size = new Size(240, 275);
            currenciesGrid.Location = new Point(60, 70);
            currenciesGrid.AllowUserToAddRows = false;
            currenciesGrid.AllowUserToDeleteRows = false;
            currenciesGrid.AllowUserToResizeRows = false;
            currenciesGrid.AllowUserToResizeColumns = false;
            currenciesGrid.AllowUserToResizeRows = false;
            currenciesGrid.MultiSelect = false;
            currenciesGrid.ReadOnly = true;
            currenciesGrid.ShowEditingIcon = false;
            currenciesGrid.ScrollBars = ScrollBars.Vertical;


            DataGridViewTextBoxColumn rankCurrenciesColumn = new DataGridViewTextBoxColumn();
            rankCurrenciesColumn.Name = "rankCurrenciesColumn";
            rankCurrenciesColumn.HeaderText = "Rank";
            currenciesGrid.Columns.Add(rankCurrenciesColumn);

            DataGridViewTextBoxColumn CurrencyCurrenciesColumn = new DataGridViewTextBoxColumn();
            CurrencyCurrenciesColumn.Name = "CurrencyCurrenciesColumn";
            CurrencyCurrenciesColumn.HeaderText = "Currency";
            currenciesGrid.Columns.Add(CurrencyCurrenciesColumn);


            pairsGrid = new MetroGrid();
            pairsGrid.Name = "pairsGrid";
            pairsGrid.Size = new Size(240, 275);
            pairsGrid.Location = new Point(390, 70);
            pairsGrid.AllowUserToAddRows = false;
            pairsGrid.AllowUserToDeleteRows = false;
            pairsGrid.AllowUserToResizeRows = false;
            pairsGrid.AllowUserToResizeColumns = false;
            pairsGrid.AllowUserToResizeRows = false;
            pairsGrid.MultiSelect = false;
            pairsGrid.ReadOnly = true;
            pairsGrid.ShowEditingIcon = false;
            pairsGrid.ScrollBars = ScrollBars.Vertical;


            DataGridViewTextBoxColumn rankPairsColumn = new DataGridViewTextBoxColumn();
            rankPairsColumn.Name = "rankPairsColumn";
            rankPairsColumn.HeaderText = "Rank";
            pairsGrid.Columns.Add(rankPairsColumn);

            DataGridViewTextBoxColumn SymbolPairsColumn = new DataGridViewTextBoxColumn();
            SymbolPairsColumn.Name = "SymbolPairsColumn";
            SymbolPairsColumn.HeaderText = "Symbol";
            pairsGrid.Columns.Add(SymbolPairsColumn);



            correlationGrid = new MetroGrid();
            correlationGrid.Name = "correlationGrid";
            correlationGrid.Size = new Size(308, 274);
            correlationGrid.Location = new Point(33, 79);
            correlationGrid.AllowUserToAddRows = false;
            correlationGrid.AllowUserToDeleteRows = false;
            correlationGrid.AllowUserToResizeRows = false;
            correlationGrid.AllowUserToResizeColumns = false;
            correlationGrid.AllowUserToResizeRows = false;
            correlationGrid.MultiSelect = false;
            correlationGrid.ReadOnly = true;
            correlationGrid.ShowEditingIcon = false;
            correlationGrid.ScrollBars = ScrollBars.Vertical;


            DataGridViewTextBoxColumn correlationSymbolsColumn = new DataGridViewTextBoxColumn();
            correlationSymbolsColumn.Name = "correlationSymbolsColumn";
            correlationSymbolsColumn.HeaderText = "Symbol";
            correlationGrid.Columns.Add(correlationSymbolsColumn);

            DataGridViewTextBoxColumn correlationNumbersColumn = new DataGridViewTextBoxColumn();
            correlationNumbersColumn.Name = "correlationNumbersColumn";
            correlationNumbersColumn.HeaderText = "Correlation(%)";
            correlationGrid.Columns.Add(correlationNumbersColumn);


            volatilityGrid = new MetroGrid();
            volatilityGrid.Name = "volatilityGrid";
            volatilityGrid.Size = new Size(590, 368);
            volatilityGrid.Location = new Point(56, 14);
            volatilityGrid.AllowUserToAddRows = false;
            volatilityGrid.AllowUserToDeleteRows = false;
            volatilityGrid.AllowUserToResizeRows = false;
            volatilityGrid.AllowUserToResizeColumns = false;
            volatilityGrid.AllowUserToResizeRows = false;
            volatilityGrid.MultiSelect = false;
            volatilityGrid.ReadOnly = true;
            volatilityGrid.ScrollBars = ScrollBars.Vertical;
            volatilityGrid.ShowEditingIcon = false;



            DataGridViewTextBoxColumn volatilitySymbolsColumn = new DataGridViewTextBoxColumn();
            volatilitySymbolsColumn.Name = "volatilitySymbolsColumn";
            volatilitySymbolsColumn.HeaderText = "Symbol";
            volatilityGrid.Columns.Add(volatilitySymbolsColumn);

            DataGridViewTextBoxColumn volatilitySDColumn = new DataGridViewTextBoxColumn();
            volatilitySDColumn.Name = "volatilitySDColumn";
            volatilitySDColumn.HeaderText = "SD(Pips)";
            volatilityGrid.Columns.Add(volatilitySDColumn);

            DataGridViewTextBoxColumn volatilityATRColumn = new DataGridViewTextBoxColumn();
            volatilityATRColumn.Name = "volatilityATRColumn";
            volatilityATRColumn.HeaderText = "ATR(Pips)";
            volatilityGrid.Columns.Add(volatilityATRColumn);

            DataGridViewTextBoxColumn volatilityABRColumn = new DataGridViewTextBoxColumn();
            volatilityABRColumn.Name = "volatilityABRColumn";
            volatilityABRColumn.HeaderText = "ABR(Pips)";
            volatilityGrid.Columns.Add(volatilityABRColumn);

            DataGridViewTextBoxColumn volatilityChaikinColumn = new DataGridViewTextBoxColumn();
            volatilityChaikinColumn.Name = "volatilityChaikinColumn";
            volatilityChaikinColumn.HeaderText = "Chaikin";
            volatilityGrid.Columns.Add(volatilityChaikinColumn);


            trendGrid = new MetroGrid();
            trendGrid.Name = "trendGrid";
            trendGrid.Size = new Size(590, 368);
            trendGrid.Location = new Point(56, 14);
            trendGrid.AllowUserToAddRows = false;
            trendGrid.AllowUserToDeleteRows = false;
            trendGrid.AllowUserToResizeRows = false;
            trendGrid.AllowUserToResizeColumns = false;
            trendGrid.AllowUserToResizeRows = false;
            trendGrid.MultiSelect = false;
            trendGrid.ReadOnly = true;
            trendGrid.ScrollBars = ScrollBars.Vertical;
            trendGrid.ShowEditingIcon = false;

            DataGridViewTextBoxColumn trendSymbolsColumn = new DataGridViewTextBoxColumn();
            trendSymbolsColumn.Name = "trendSymbolsColumn";
            trendSymbolsColumn.HeaderText = "Symbol";
            trendGrid.Columns.Add(trendSymbolsColumn);

            DataGridViewTextBoxColumn trendMAColumn = new DataGridViewTextBoxColumn();
            trendMAColumn.Name = "trendMAColumn";
            trendMAColumn.HeaderText = "MA";
            trendGrid.Columns.Add(trendMAColumn);

            DataGridViewTextBoxColumn trendRSIColumn = new DataGridViewTextBoxColumn();
            trendRSIColumn.Name = "trendRSIColumn";
            trendRSIColumn.HeaderText = "RSI";
            trendGrid.Columns.Add(trendRSIColumn);

            DataGridViewTextBoxColumn trendMACDColumn = new DataGridViewTextBoxColumn();
            trendMACDColumn.Name = "trendMACDColumn";
            trendMACDColumn.HeaderText = "MACD";
            trendGrid.Columns.Add(trendMACDColumn);

            DataGridViewTextBoxColumn trendTrendColumn = new DataGridViewTextBoxColumn();
            trendTrendColumn.Name = "trendTrendColumn";
            trendTrendColumn.HeaderText = "Trend";
            trendGrid.Columns.Add(trendTrendColumn);


            // Adding Process
            startTab.Controls.AddRange(new Control[] 
            {
                labelStartWelcome,
                labelStartSeriesTF,
                labelStartSWSMAPeriods,
                labelStartCorrelationPeriods,
                labelStartSDPeriods,
                labelStartATRPeriods,
                labelStartABRPeriods,
                labelStartChaikinVolatilityPeriods,
                labelStartChaikinRateChange,
                labelStartUpdate,
                labelStartMovingAverageType,
                labelStartMovingAveragePeriods,
                labelStartRSIPeriods,
                labelStartRSITrendSeperator,
                labelStartMACDLongCycle,
                labelStartMACDShortCycle,
                labelStartMACDSignalPeriods,
                swSMAPeriods,
                correlationPeriods,
                sdPeriods,
                atrPeriods,
                abrPeriods,
                chaikinPeriods,
                chaikinRate,
                comboBoxSeriesTF,
                comboBoxMaType,
                maPeriods,
                rsiPeriods,
                rsiSeperator,
                macdLongCycle,
                macdShortCycle,
                macdSignalPeriods,
                updateMinutes,
                symbolsButton,
                updateButton,
                sortSettingsButton

            });


            strongWeakTab.Controls.AddRange(new Control[] 
            {
                labelCurrencies,
                labelPairs,
                currenciesGrid,
                pairsGrid,
                dailyFXLink
            });

            correlationTab.Controls.AddRange(new Control[] 
            {
                labelCorrelationSelectSymbol,
                comboBoxCorrelationSymbols,
                correlationGrid,
                labelCorrelationDescription,
                investopediaLink
            });

            volatilityTab.Controls.AddRange(new Control[] 
            {
                volatilityGrid
            });

            trendTab.Controls.AddRange(new Control[] 
            {
                trendGrid
            });

            aboutTab.Controls.AddRange(new Control[] 
            {
                picBox,
                labelAboutDeveloper,
                labelAboutVersion,
                labelAboutContact,
                labelAboutEmail,
                txtBoxAboutEmail,
                labelAboutSkype,
                txtBoxAboutSkype,
                labelAboutDonation,
                labelAboutBitcoin,
                txtBoxAboutBitcoin,
                labelAboutPaypal,
                txtBoxAboutPaypal,
                ctdnLink
            });

            Tabs.TabPages.AddRange(new TabPage[] 
            {
                startTab,
                strongWeakTab,
                correlationTab,
                volatilityTab,
                trendTab,
                aboutTab
            });

            DashboardForm.Controls.Add(LabelWait);
            DashboardForm.Controls.Add(Tabs);
            DashboardForm.Controls.Add(SymbolsProgressBar);

            Application.Run(DashboardForm);
        }

        private void SymbolsFormInitializer()
        {
            SymbolsForm = new MetroForm();
            SymbolsForm.Name = "SymbolsForm";
            SymbolsForm.Text = "Symbols";
            SymbolsForm.Size = new Size(329, 343);
            SymbolsForm.MaximizeBox = false;
            SymbolsForm.ShowInTaskbar = false;
            SymbolsForm.StartPosition = FormStartPosition.CenterParent;
            SymbolsForm.Resizable = false;
            SymbolsForm.ShowIcon = false;
            SymbolsForm.Theme = MetroFramework.MetroThemeStyle.Dark;
            SymbolsForm.Style = MetroFramework.MetroColorStyle.Silver;
            SymbolsForm.Load += new EventHandler(SymbolsFormLoad);
            SymbolsForm.FormClosed += new FormClosedEventHandler(SymbolsFormClosed);


            symbolTextBox = new MetroTextBox();
            symbolTextBox.Name = "symbolTextBox";
            symbolTextBox.Text = "Enter Symbol Code";
            symbolTextBox.Size = new Size(145, 23);
            symbolTextBox.Location = new Point(46, 63);

            MetroButton addSymbolButton = new MetroButton();
            addSymbolButton.Name = "addSymbolButton";
            addSymbolButton.Text = "Add Symbol";
            addSymbolButton.Size = new Size(75, 23);
            addSymbolButton.Location = new Point(206, 63);
            addSymbolButton.Click += new EventHandler(AddSymbol);

            symbolsGrid = new MetroGrid();
            symbolsGrid.Name = "symbolsGrid";
            symbolsGrid.Size = new Size(235, 191);
            symbolsGrid.Location = new Point(46, 104);
            symbolsGrid.AllowUserToAddRows = false;
            symbolsGrid.AllowUserToDeleteRows = false;
            symbolsGrid.AllowUserToResizeRows = false;
            symbolsGrid.AllowUserToResizeColumns = false;
            symbolsGrid.AllowUserToResizeRows = false;
            symbolsGrid.ReadOnly = true;
            symbolsGrid.ShowEditingIcon = false;
            symbolsGrid.ScrollBars = ScrollBars.Vertical;


            DataGridViewTextBoxColumn currentSymbolsColumn = new DataGridViewTextBoxColumn();
            currentSymbolsColumn.Name = "currentSymbolsColumn";
            currentSymbolsColumn.HeaderText = "Current Symbols";
            currentSymbolsColumn.Width = 150;
            currentSymbolsColumn.FillWeight = 150;
            symbolsGrid.Columns.Add(currentSymbolsColumn);

            deleteSymbolsButton = new MetroButton();
            deleteSymbolsButton.Name = "deleteSymbolsButton";
            deleteSymbolsButton.Text = "Delete Selected Symbols";
            deleteSymbolsButton.Size = new Size(145, 23);
            deleteSymbolsButton.Location = new Point(46, 313);
            deleteSymbolsButton.Click += new EventHandler(DeleteSymbols);

            MetroButton applyButton = new MetroButton();
            applyButton.Name = "applyButton";
            applyButton.Text = "Apply";
            applyButton.Size = new Size(75, 23);
            applyButton.Location = new Point(206, 313);
            applyButton.Click += new EventHandler(ApplySymbolsChange);


            SymbolsForm.Controls.AddRange(new Control[] 
            {
                symbolTextBox,
                addSymbolButton,
                symbolsGrid,
                deleteSymbolsButton,
                applyButton
            });

            foreach (string sym in allSymbols)
            {
                symbolsGrid.Rows.Add(sym);
            }

            symbolsGrid.PerformLayout();


            //Application.Run(SymbolsForm);
            SymbolsForm.ShowDialog(DashboardForm);
        }

        private void SortSettingsFormInitializer()
        {
            sortSettingsForm = new MetroForm();
            correlationGridLabel = new MetroLabel();
            trendGridLabel = new MetroLabel();
            volatilityGridLabel = new MetroLabel();
            correlationColumnsComboBox = new MetroComboBox();
            crrelationSortDirectionComboBox = new MetroComboBox();
            volatilityColumnComboBox = new MetroComboBox();
            volatilitySortDirectionComboBox = new MetroComboBox();
            trendColumnsComboBox = new MetroComboBox();
            trendSortDirectionComboBox = new MetroComboBox();
            sortSettingOkButton = new MetroButton();
            sortSettingsForm.SuspendLayout();
            // 
            // correlationGridLabel
            // 
            correlationGridLabel.AutoSize = true;
            correlationGridLabel.Location = new Point(23, 77);
            correlationGridLabel.Name = "correlationGridLabel";
            correlationGridLabel.Size = new Size(110, 20);
            correlationGridLabel.TabIndex = 0;
            correlationGridLabel.Text = "Correlation Grid:";
            correlationGridLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // trendGridLabel
            // 
            trendGridLabel.AutoSize = true;
            trendGridLabel.Location = new Point(23, 176);
            trendGridLabel.Name = "trendGridLabel";
            trendGridLabel.Size = new Size(77, 20);
            trendGridLabel.TabIndex = 1;
            trendGridLabel.Text = "Trend Grid:";
            trendGridLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // volatilityGridLabel
            // 
            volatilityGridLabel.AutoSize = true;
            volatilityGridLabel.Location = new Point(23, 127);
            volatilityGridLabel.Name = "volatilityGridLabel";
            volatilityGridLabel.Size = new Size(92, 20);
            volatilityGridLabel.TabIndex = 2;
            volatilityGridLabel.Text = "Volatility Grid:";
            volatilityGridLabel.Theme = MetroFramework.MetroThemeStyle.Dark;
            // 
            // correlationColumnsComboBox
            // 
            correlationColumnsComboBox.FormattingEnabled = true;
            correlationColumnsComboBox.ItemHeight = 24;
            correlationColumnsComboBox.Items.AddRange(new object[] 
            {
                "Symbol",
                "Correlation(%)"
            });
            correlationColumnsComboBox.Location = new Point(164, 67);
            correlationColumnsComboBox.Name = "correlationColumnsComboBox";
            correlationColumnsComboBox.Size = new Size(121, 30);
            correlationColumnsComboBox.TabIndex = 3;
            correlationColumnsComboBox.UseSelectable = true;
            correlationColumnsComboBox.SelectedIndex = correlationGridSortColumnIndex;
            // 
            // crrelationSortDirectionComboBox
            // 
            crrelationSortDirectionComboBox.FormattingEnabled = true;
            crrelationSortDirectionComboBox.ItemHeight = 24;
            crrelationSortDirectionComboBox.Items.AddRange(new object[] 
            {
                "Ascending",
                "Descending"
            });
            crrelationSortDirectionComboBox.Location = new Point(307, 67);
            crrelationSortDirectionComboBox.Name = "crrelationSortDirectionComboBox";
            crrelationSortDirectionComboBox.Size = new Size(121, 30);
            crrelationSortDirectionComboBox.TabIndex = 4;
            crrelationSortDirectionComboBox.UseSelectable = true;

            if (correlationGridSortDirection == ListSortDirection.Ascending)
                crrelationSortDirectionComboBox.SelectedIndex = 0;
            else
                crrelationSortDirectionComboBox.SelectedIndex = 1;

            // 
            // volatilityColumnComboBox
            // 
            volatilityColumnComboBox.FormattingEnabled = true;
            volatilityColumnComboBox.ItemHeight = 24;
            volatilityColumnComboBox.Items.AddRange(new object[] 
            {
                "Symbol",
                "SD(Pips)",
                "ATR(Pips)",
                "ABR(Pips)",
                "Chaikin"
            });
            volatilityColumnComboBox.Location = new Point(164, 117);
            volatilityColumnComboBox.Name = "volatilityColumnComboBox";
            volatilityColumnComboBox.Size = new Size(121, 30);
            volatilityColumnComboBox.TabIndex = 5;
            volatilityColumnComboBox.UseSelectable = true;
            volatilityColumnComboBox.SelectedIndex = volatilityGridSortColumnIndex;
            // 
            // volatilitySortDirectionComboBox
            // 
            volatilitySortDirectionComboBox.FormattingEnabled = true;
            volatilitySortDirectionComboBox.ItemHeight = 24;
            volatilitySortDirectionComboBox.Items.AddRange(new object[] 
            {
                "Ascending",
                "Descending"
            });
            volatilitySortDirectionComboBox.Location = new Point(307, 117);
            volatilitySortDirectionComboBox.Name = "volatilitySortDirectionComboBox";
            volatilitySortDirectionComboBox.Size = new Size(121, 30);
            volatilitySortDirectionComboBox.TabIndex = 6;
            volatilitySortDirectionComboBox.UseSelectable = true;

            if (volatilityGridSortDirection == ListSortDirection.Ascending)
                volatilitySortDirectionComboBox.SelectedIndex = 0;
            else
                volatilitySortDirectionComboBox.SelectedIndex = 1;
            // 
            // trendColumnsComboBox
            // 
            trendColumnsComboBox.FormattingEnabled = true;
            trendColumnsComboBox.ItemHeight = 24;
            trendColumnsComboBox.Items.AddRange(new object[] 
            {
                "Symbol",
                "MA",
                "RSI",
                "MACD",
                "Trend"
            });
            trendColumnsComboBox.Location = new Point(164, 166);
            trendColumnsComboBox.Name = "trendColumnsComboBox";
            trendColumnsComboBox.Size = new Size(121, 30);
            trendColumnsComboBox.TabIndex = 7;
            trendColumnsComboBox.UseSelectable = true;
            trendColumnsComboBox.SelectedIndex = trendGridSortColumnIndex;
            // 
            // trendSortDirectionComboBox
            // 
            trendSortDirectionComboBox.FormattingEnabled = true;
            trendSortDirectionComboBox.ItemHeight = 24;
            trendSortDirectionComboBox.Items.AddRange(new object[] 
            {
                "Ascending",
                "Descending"
            });
            trendSortDirectionComboBox.Location = new Point(307, 166);
            trendSortDirectionComboBox.Name = "trendSortDirectionComboBox";
            trendSortDirectionComboBox.Size = new Size(121, 30);
            trendSortDirectionComboBox.TabIndex = 8;
            trendSortDirectionComboBox.UseSelectable = true;
            if (trendGridSortDirection == ListSortDirection.Ascending)
                trendSortDirectionComboBox.SelectedIndex = 0;
            else
                trendSortDirectionComboBox.SelectedIndex = 1;
            // 
            // sortSettingOkButton
            // 
            sortSettingOkButton.Location = new Point(179, 213);
            sortSettingOkButton.Name = "sortSettingOkButton";
            sortSettingOkButton.Size = new Size(75, 23);
            sortSettingOkButton.TabIndex = 9;
            sortSettingOkButton.Text = "Ok";
            sortSettingOkButton.UseSelectable = true;
            sortSettingOkButton.Click += new EventHandler(SortSettingOkButtonClicked);
            // 
            // SortForm
            // 
            sortSettingsForm.AutoScaleDimensions = new SizeF(8f, 16f);
            sortSettingsForm.AutoScaleMode = AutoScaleMode.Font;
            sortSettingsForm.ClientSize = new Size(445, 250);
            sortSettingsForm.Controls.Add(sortSettingOkButton);
            sortSettingsForm.Controls.Add(trendSortDirectionComboBox);
            sortSettingsForm.Controls.Add(trendColumnsComboBox);
            sortSettingsForm.Controls.Add(volatilitySortDirectionComboBox);
            sortSettingsForm.Controls.Add(volatilityColumnComboBox);
            sortSettingsForm.Controls.Add(crrelationSortDirectionComboBox);
            sortSettingsForm.Controls.Add(correlationColumnsComboBox);
            sortSettingsForm.Controls.Add(volatilityGridLabel);
            sortSettingsForm.Controls.Add(trendGridLabel);
            sortSettingsForm.Controls.Add(correlationGridLabel);
            sortSettingsForm.MaximizeBox = false;
            sortSettingsForm.MinimizeBox = false;
            sortSettingsForm.Name = "sortSettingsForm";
            sortSettingsForm.Resizable = false;
            sortSettingsForm.ShowIcon = false;
            sortSettingsForm.Text = "Sort Settings";
            sortSettingsForm.Theme = MetroFramework.MetroThemeStyle.Dark;
            sortSettingsForm.Style = MetroFramework.MetroColorStyle.Silver;
            sortSettingsForm.FormClosed += new FormClosedEventHandler(SortSettingsFormClosed);
            sortSettingsForm.Load += new EventHandler(SortSettingsFormLoad);
            sortSettingsForm.StartPosition = FormStartPosition.CenterParent;
            sortSettingsForm.ShowInTaskbar = false;

            sortSettingsForm.ResumeLayout(false);
            sortSettingsForm.PerformLayout();

            sortSettingsForm.ShowDialog(DashboardForm);
        }


        // Strong Weak Start
        private void StrongWeakOperation()
        {
            Dictionary<int, string> strongCurrencies = Strong();

            if (currenciesGrid.Rows.Count > 0)
                currenciesGrid.Rows.Clear();
            if (pairsGrid.Rows.Count > 0)
                pairsGrid.Rows.Clear();


            foreach (KeyValuePair<int, string> item in strongCurrencies)
            {
                currenciesGrid.Rows.Add(item.Key, item.Value);
            }

            currenciesGrid.PerformLayout();

            Dictionary<int, string> weakCurrencies = new Dictionary<int, string>();
            int weakKey = 1;
            for (int i = strongCurrencies.Count; i > 0; i--)
            {
                if (strongCurrencies.ContainsKey(i))
                {
                    weakCurrencies.Add(weakKey, strongCurrencies[i]);
                    weakKey += 1;
                }
            }


            int strongPairKey = 1;
            foreach (KeyValuePair<int, string> strongItem in strongCurrencies)
            {
                List<string> currencySymbols = currenciesWithSymbols[strongItem.Value];
                foreach (string sym in currencySymbols)
                {
                    if (sym.StartsWith(strongItem.Value))
                    {
                        foreach (KeyValuePair<int, string> weakItem in weakCurrencies)
                        {
                            if (sym.EndsWith(weakItem.Value) && weakItem.Key <= strongItem.Key)
                            {
                                pairsGrid.Rows.Add(strongPairKey, sym);
                                strongPairKey += 1;
                            }
                        }
                    }
                }
            }

            pairsGrid.PerformLayout();
        }


        private Dictionary<int, string> Strong()
        {
            if (currenciesWithSymbols.Count > 0)
                currenciesWithSymbols.Clear();

            foreach (string currency in currencies)
            {
                List<string> currencySymbols = new List<string>();
                foreach (string sym in allSymbols)
                {
                    if (sym.StartsWith(currency) || sym.EndsWith(currency))
                        currencySymbols.Add(sym);
                }

                currenciesWithSymbols.Add(currency, currencySymbols);
            }

            Dictionary<int, string> resultStrong = new Dictionary<int, string>();
            Dictionary<string, int> forStrongWhile = CalculateSMA();

            List<string> itemsForRemoveStrong = new List<string>();
            bool endStrongWhile = false;
            int rankStrong = 1;
            while (!endStrongWhile)
            {
                foreach (KeyValuePair<string, int> item in forStrongWhile)
                {
                    bool greater = true;

                    foreach (KeyValuePair<string, int> secondItem in forStrongWhile)
                    {
                        if (item.Value < secondItem.Value)
                        {
                            greater = false;
                            break;
                        }
                    }

                    if (greater)
                    {
                        resultStrong.Add(rankStrong, item.Key);
                        itemsForRemoveStrong.Add(item.Key);
                        rankStrong += 1;
                    }
                }

                foreach (string item in itemsForRemoveStrong)
                {
                    forStrongWhile.Remove(item);
                }



                if (forStrongWhile.Count == 0)
                    endStrongWhile = true;
            }


            return resultStrong;
        }


        private Dictionary<string, int> CalculateSMA()
        {
            Dictionary<string, int> currenciesWithValues = new Dictionary<string, int>();

            foreach (KeyValuePair<string, List<string>> item in currenciesWithSymbols)
            {
                int currencyValue = 0;
                foreach (string sym in item.Value)
                {
                    MarketSeries symSeries = SymbolsSeries[sym];
                    SimpleMovingAverage sma = Indicators.SimpleMovingAverage(symSeries.Close, TextToInt(swSMAPeriods));

                    if (sym.StartsWith(item.Key) && symSeries.Close.Last(1) > sma.Result.Last(1))
                        currencyValue += 1;
                    else if (sym.StartsWith(item.Key) && symSeries.Close.Last(1) < sma.Result.Last(1))
                        currencyValue -= 1;
                    else if (sym.EndsWith(item.Key) && symSeries.Close.Last(1) > sma.Result.Last(1))
                        currencyValue -= 1;
                    else if (sym.EndsWith(item.Key) && symSeries.Close.Last(1) < sma.Result.Last(1))
                        currencyValue += 1;
                }

                currenciesWithValues.Add(item.Key, currencyValue);
            }

            return currenciesWithValues;
        }

        // Strong Weak End



        // Volatility Start
        private void VolatilityOperation()
        {
            if (volatilityGrid.Rows.Count > 0)
                volatilityGrid.Rows.Clear();

            foreach (string symCode in allSymbols)
            {
                MarketSeries symSeries = SymbolsSeries[symCode];
                Symbol sym = MarketData.GetSymbol(symCode);
                StandardDeviation sd = Indicators.StandardDeviation(symSeries.Close, TextToInt(sdPeriods), MovingAverageType.Simple);
                AverageTrueRange atr = Indicators.AverageTrueRange(symSeries, TextToInt(atrPeriods), MovingAverageType.Exponential);
                ChaikinVolatility chaikin = Indicators.ChaikinVolatility(symSeries, TextToInt(chaikinPeriods), TextToInt(chaikinRate), MovingAverageType.Simple);

                double sdPips = Math.Round(sd.Result.Last(1) * Math.Pow(10, sym.Digits - 1), 1);
                double atrPips = Math.Round(atr.Result.Last(1) * Math.Pow(10, sym.Digits - 1), 1);
                double abrPips = ABR(TextToInt(abrPeriods), symSeries, sym);
                double chaikinRounded = Math.Round(chaikin.Result.Last(1), 1);

                volatilityGrid.Rows.Add(symCode, sdPips, atrPips, abrPips, chaikinRounded);
            }

            volatilityGrid.Sort(volatilityGrid.Columns[volatilityGridSortColumnIndex], volatilityGridSortDirection);
            volatilityGrid.PerformLayout();


        }


        private double ABR(int periods, MarketSeries symbolSeries, Symbol sym)
        {
            double adr = 0;
            for (int bar = 1; bar <= periods; bar++)
            {
                adr += (symbolSeries.High.Last(bar) - symbolSeries.Low.Last(bar)) * Math.Pow(10, sym.Digits - 1);
            }

            return Math.Round((adr /= periods), 1);
        }

        // Volatility End




        // Correlation Start
        public void ComputeCorrelation(string selectedSymbol)
        {
            MarketSeries selectedSymbolSeries = SymbolsSeries[selectedSymbol];

            Dictionary<string, double> correlationResult = new Dictionary<string, double>();
            foreach (string sym in allSymbols)
            {
                int intCorrelationPeriods = TextToInt(correlationPeriods);
                MarketSeries symSeries = SymbolsSeries[sym];
                if (selectedSymbolSeries.Close.Count < intCorrelationPeriods && symSeries.Close.Count < intCorrelationPeriods)
                    continue;

                double[] values1 = new double[intCorrelationPeriods];
                double[] values2 = new double[intCorrelationPeriods];

                for (int i = 0; i < intCorrelationPeriods; i++)
                {
                    values1[i] = selectedSymbolSeries.Close.Last(i + 1);
                    values2[i] = symSeries.Close.Last(i + 1);
                }

                var avg1 = values1.Average();
                var avg2 = values2.Average();

                var sum1 = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

                var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
                var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

                double result = Math.Round(sum1 / Math.Sqrt(sumSqr1 * sumSqr2), 2) * 100;

                correlationResult.Add(sym, result);
            }


            if (correlationGrid.Rows.Count > 0)
                correlationGrid.Rows.Clear();

            foreach (KeyValuePair<string, double> item in correlationResult)
            {
                if (item.Key == selectedSymbol)
                    continue;

                correlationGrid.Rows.Add(item.Key, item.Value);
            }

            correlationGrid.Sort(correlationGrid.Columns[correlationGridSortColumnIndex], correlationGridSortDirection);
            correlationGrid.PerformLayout();
            comboBoxCorrelationSymbols.Enabled = true;
        }


        private void CorrelationUpdate()
        {
            comboBoxCorrelationSymbols.Items.Clear();
            foreach (string sym in allSymbols)
            {
                comboBoxCorrelationSymbols.Items.Add(sym);
            }

            correlationGrid.Rows.Clear();

        }

        // Correlation End


        // Trend Start

        private void Trend()
        {
            if (trendGrid.Rows.Count > 0)
                trendGrid.Rows.Clear();

            foreach (string symCode in allSymbols)
            {
                MovingAverageType maType;
                if (comboBoxMaType.SelectedItem.ToString() == "Exponential")
                    maType = MovingAverageType.Exponential;
                else
                    maType = MovingAverageType.Simple;


                MarketSeries symSeries = SymbolsSeries[symCode];
                MovingAverage ma = Indicators.MovingAverage(symSeries.Close, TextToInt(maPeriods), maType);
                RelativeStrengthIndex rsi = Indicators.RelativeStrengthIndex(symSeries.Close, TextToInt(rsiPeriods));
                MacdCrossOver macd = Indicators.MacdCrossOver(symSeries.Close, TextToInt(macdLongCycle), TextToInt(macdShortCycle), TextToInt(macdSignalPeriods));


                string maResult;
                string rsiResult;
                string macdResult;
                string trendResult;

                if (symSeries.Close.Last(1) > ma.Result.Last(1))
                    maResult = "Up";
                else if (symSeries.Close.Last(1) < ma.Result.Last(1))
                    maResult = "Down";
                else
                    maResult = "Neutral";

                if (rsi.Result.Last(1) > TextToInt(rsiSeperator))
                    rsiResult = "Up";
                else if (rsi.Result.Last(1) < TextToInt(rsiSeperator))
                    rsiResult = "Down";
                else
                    rsiResult = "Neutral";

                if (macd.MACD.Last(1) > macd.Signal.Last(1))
                    macdResult = "Up";
                else if (macd.MACD.Last(1) < macd.Signal.Last(1))
                    macdResult = "Down";
                else
                    macdResult = "Neutral";

                if (maResult == "Up" && rsiResult == "Up" && macdResult == "Up")
                    trendResult = "Up";
                else if (maResult == "Down" && rsiResult == "Down" && macdResult == "Down")
                    trendResult = "Down";
                else
                    trendResult = "Neutral";

                trendGrid.Rows.Add(symCode, maResult, rsiResult, macdResult, trendResult);
            }

            trendGrid.Sort(trendGrid.Columns[trendGridSortColumnIndex], trendGridSortDirection);
            trendGrid.PerformLayout();
        }

        // Trend End






        // Updater
        private void Updater()
        {
            if (IsFormOpen("DashboardForm"))
            {
                checkFormTimer.Stop();
                Timer.Stop();

                Task updateTask = Task.Factory.StartNew(() =>
                {
                    if (IsFormOpen("SymbolsForm"))
                        SymbolsForm.Close();



                    Tabs.Enabled = false;
                    LabelWait.Visible = true;
                    SymbolsProgressBar.Value = 0;
                    SymbolsProgressBar.Visible = true;


                    if (SymbolsSeries.Count > 0)
                    {
                        SymbolsSeries.Clear();
                        RefreshData();
                    }

                    TimeFrame seriesTF = SeriesTimeFrames[comboBoxSeriesTF.SelectedItem.ToString()];

                    foreach (string sym in allSymbols)
                    {
                        MarketSeries symSeries = MarketData.GetSeries(sym, seriesTF);
                        SymbolsSeries.Add(sym, symSeries);
                        SymbolsProgressBar.Value += 1;
                    }


                    StrongWeakOperation();
                    VolatilityOperation();
                    Trend();
                    CorrelationUpdate();

                    Timer.Start(TextToInt(updateMinutes) * 60);

                    LabelWait.Visible = false;
                    SymbolsProgressBar.Visible = false;
                    Tabs.Enabled = true;
                });

            }
        }




        // cBot Methods

        protected override void OnTimer()
        {
            Updater();
        }

        protected override void OnStop()
        {
            DashboardForm.Close();
        }





        // Event Handlers

        private void OpenInvestopediaLink(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.investopedia.com/terms/c/correlation.asp");
        }

        private void OpenDailyFXLink(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.dailyfx.com/forex/education/trading_tips/post_of_the_day/2011/06/15/How_to_Create_a_Trading_Edge_Know_the_Strong_and_the_Weak_Currencies.html");
        }

        private void OpencTDNLink(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://ctdn.com/users/profile/afhacker");
        }

        private void CheckForm(object sender, ElapsedEventArgs e)
        {
            Updater();
        }

        private void DashboardFormLoad(object sender, EventArgs e)
        {
            DashboardForm.TopMost = true;
        }

        private void DashboardFormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void SymbolsFormLoad(object sender, EventArgs e)
        {
            Tabs.Enabled = false;
            DashboardForm.TopMost = false;
            SymbolsForm.TopMost = true;
        }

        private void SymbolsFormClosed(object sender, FormClosedEventArgs e)
        {
            Tabs.Enabled = true;
            DashboardForm.TopMost = true;
        }

        private void SortSettingsFormLoad(object sender, EventArgs e)
        {
            Tabs.Enabled = false;
            DashboardForm.TopMost = false;
            sortSettingsForm.TopMost = true;
        }

        private void SortSettingsFormClosed(object sender, FormClosedEventArgs e)
        {
            Tabs.Enabled = true;
            DashboardForm.TopMost = true;
            Updater();
        }

        private void comboBoxSymbolSelected(object sender, EventArgs e)
        {
            comboBoxCorrelationSymbols.Enabled = false;
            ComputeCorrelation(comboBoxCorrelationSymbols.SelectedItem.ToString());
        }

        private void UpdateClicked(object sender, EventArgs e)
        {
            Updater();
        }

        private void SymbolsClicked(object sender, EventArgs e)
        {
            Task showSymbolsForm = Task.Factory.StartNew(() => { SymbolsFormInitializer(); });
        }

        private void SortSettingsClicked(object sender, EventArgs e)
        {
            Task showSortSettingsForm = Task.Factory.StartNew(() => { SortSettingsFormInitializer(); });
        }

        private void AddSymbol(object sender, EventArgs e)
        {
            if (symbolTextBox.Text.Length == 6)
                symbolsGrid.Rows.Add(symbolTextBox.Text);
            symbolsGrid.PerformLayout();
        }

        private void DeleteSymbols(object sender, EventArgs e)
        {
            deleteSymbolsButton.Enabled = false;

            foreach (DataGridViewRow row in symbolsGrid.SelectedRows)
            {
                symbolsGrid.Rows.Remove(row);
            }

            List<DataGridViewRow> rowsCopy = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in symbolsGrid.Rows)
            {
                rowsCopy.Add(row);
            }

            symbolsGrid.Rows.Clear();

            foreach (DataGridViewRow row in rowsCopy)
            {
                symbolsGrid.Rows.Add(row);
            }

            symbolsGrid.PerformLayout();
            symbolsGrid.Refresh();
            deleteSymbolsButton.Enabled = true;
        }

        private void ApplySymbolsChange(object sender, EventArgs e)
        {
            if (allSymbols.Count > 0)
                allSymbols.Clear();

            foreach (DataGridViewRow row in symbolsGrid.Rows)
            {
                string sym = row.Cells[0].Value.ToString();
                if (!allSymbols.Contains(sym))
                    allSymbols.Add(sym);
            }

            FillCurrencies();
            Updater();
        }

        private void SortSettingOkButtonClicked(object sender, EventArgs e)
        {
            correlationGridSortColumnIndex = correlationColumnsComboBox.SelectedIndex;
            volatilityGridSortColumnIndex = volatilityColumnComboBox.SelectedIndex;
            trendGridSortColumnIndex = trendColumnsComboBox.SelectedIndex;

            if (crrelationSortDirectionComboBox.SelectedIndex == 0)
                correlationGridSortDirection = ListSortDirection.Ascending;
            else
                correlationGridSortDirection = ListSortDirection.Descending;

            if (volatilitySortDirectionComboBox.SelectedIndex == 0)
                volatilityGridSortDirection = ListSortDirection.Ascending;
            else
                volatilityGridSortDirection = ListSortDirection.Descending;

            if (trendSortDirectionComboBox.SelectedIndex == 0)
                trendGridSortDirection = ListSortDirection.Ascending;
            else
                trendGridSortDirection = ListSortDirection.Descending;

            sortSettingsForm.Close();
        }


        // Extra Methods
        private int TextToInt(MetroTextBox textBox)
        {
            return int.Parse(textBox.Text);
        }

        private void FillCurrencies()
        {
            if (currencies.Count > 0)
                currencies.Clear();
            foreach (string sym in allSymbols)
            {
                string firstCurrency = sym.Substring(0, 3);
                string secondCurrency = sym.Substring(3, 3);
                if (!currencies.Contains(firstCurrency))
                    currencies.Add(firstCurrency);
                if (!currencies.Contains(secondCurrency))
                    currencies.Add(secondCurrency);
            }
        }

        private bool IsFormOpen(string formName)
        {
            FormCollection formsCollection = Application.OpenForms;
            foreach (Form frm in formsCollection)
            {
                if (frm.Name == formName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
