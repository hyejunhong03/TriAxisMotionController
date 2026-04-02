using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace project_sharp
{
    public partial class Form1 : Form
    {
        private readonly AxisController axisX = new AxisController("X");
        private readonly AxisController axisY = new AxisController("Y");
        private readonly AxisController axisZ = new AxisController("Z");

        public Form1()
        {
            InitializeComponent();
            InitializeComboBoxes();
            WireEventHandlers();
        }

        private void InitializeComboBoxes()
        {
            string[] ips = { "192.168.2.100", "192.168.2.101", "192.168.2.102" };
            foreach (string ip in ips)
            {
                comboBox1.Items.Add(ip);
                comboBox2.Items.Add(ip);
                comboBox3.Items.Add(ip);
            }
            comboBox4.Items.Add("X축");
            comboBox4.Items.Add("Y축");
            comboBox4.Items.Add("Z축");
        }

        private void WireEventHandlers()
        {
            // Connect
            button1.Click += button1_Click;
            button2.Click += button2_Click;
            button3.Click += button3_Click;
            button4.Click += button4_Click;
            button5.Click += button5_Click;
            button6.Click += button6_Click;

            // Command
            textBox1.KeyDown += textBox1_KeyDown;

            // REF
            button7.Click += button7_Click;
            button8.Click += button8_Click;
            button9.Click += button9_Click;

            // GP0
            button10.Click += button10_Click;
            button11.Click += button11_Click;
            button12.Click += button12_Click;

            // SM
            button13.Click += button13_Click;
            button14.Click += button14_Click;
            button15.Click += button15_Click;

            // PWC
            button16.Click += button16_Click;
            button17.Click += button17_Click;
            button18.Click += button18_Click;

            // PQ
            button19.Click += button19_Click;
            button20.Click += button20_Click;
            button21.Click += button21_Click;

            // ABS
            textBox17.KeyDown += textBox17_KeyDown;
            textBox21.KeyDown += textBox21_KeyDown;
            textBox25.KeyDown += textBox25_KeyDown;

            // REL
            textBox16.KeyDown += textBox16_KeyDown;
            textBox20.KeyDown += textBox20_KeyDown;
            textBox24.KeyDown += textBox24_KeyDown;

            // RR
            textBox15.KeyDown += textBox15_KeyDown;
            textBox19.KeyDown += textBox19_KeyDown;
            textBox23.KeyDown += textBox23_KeyDown;

            // WT
            textBox14.KeyDown += textBox14_KeyDown;
            textBox18.KeyDown += textBox18_KeyDown;
            textBox22.KeyDown += textBox22_KeyDown;

            // SP
            textBox2.KeyDown  += textBox2_KeyDown;
            textBox9.KeyDown  += textBox9_KeyDown;
            textBox13.KeyDown += textBox13_KeyDown;

            // AC
            textBox3.KeyDown  += textBox3_KeyDown;
            textBox8.KeyDown  += textBox8_KeyDown;
            textBox12.KeyDown += textBox12_KeyDown;

            // OVRD
            textBox4.KeyDown  += textBox4_KeyDown;
            textBox7.KeyDown  += textBox7_KeyDown;
            textBox11.KeyDown += textBox11_KeyDown;

            // S-Curve
            textBox5.KeyDown  += textBox5_KeyDown;
            textBox6.KeyDown  += textBox6_KeyDown;
            textBox10.KeyDown += textBox10_KeyDown;

            // SLP-
            textBox27.KeyDown += textBox27_KeyDown;
            textBox29.KeyDown += textBox29_KeyDown;
            textBox31.KeyDown += textBox31_KeyDown;

            // SLP+
            textBox26.KeyDown += textBox26_KeyDown;
            textBox28.KeyDown += textBox28_KeyDown;
            textBox30.KeyDown += textBox30_KeyDown;
        }


        // ─── 공통 헬퍼 ──────────────────────────────────────────────────────────

        private void Log(string message) => richTextBox1.AppendText(message + "\n");

        private AxisController GetAxisFromComboBox4()
        {
            if (comboBox4.SelectedItem == null) return null;
            switch (comboBox4.SelectedItem.ToString())
            {
                case "X축": return axisX;
                case "Y축": return axisY;
                case "Z축": return axisZ;
                default:    return null;
            }
        }

        // 단순 명령 전송 (REF, GP0, SM, PWC, PQ 등)
        private async Task SendSimpleCommandAsync(AxisController axis, string command)
        {
            if (!axis.IsConnected) { Log("IP 주소를 선택하고 연결을 설정하세요."); return; }
            try
            {
                string response = await axis.SendCommandAsync(command);
                Log(response != null ? "Received message: " + response : "서버로부터 데이터 수신 실패.");
            }
            catch (Exception ex) { Log("오류 발생: " + ex.Message); }
        }

        // 파라미터 명령 전송 (SP, AC, OVRD, SCRV, SLPN, SLPP, WT, ABS 등)
        private async Task SendParamCommandAsync(AxisController axis, string prefix, string value, Label displayLabel)
        {
            if (!axis.IsConnected) { Log("IP 주소를 선택하고 연결을 설정하세요."); return; }
            try
            {
                string response = await axis.SendCommandAsync(prefix + value);
                if (response != null) { Log("Received message: " + response); displayLabel.Text = value; }
                else Log("서버로부터 데이터 수신 실패.");
            }
            catch (Exception ex) { Log("오류 발생: " + ex.Message); }
        }

        // REL: WA 전송 후 GW 후속 명령 실행
        private async Task SendRelCommandAsync(AxisController axis, string value, Label displayLabel)
        {
            if (!axis.IsConnected) { Log("IP 주소를 선택하고 연결을 설정하세요."); return; }
            try
            {
                string waResponse = await axis.SendCommandAsync("WA" + value);
                if (waResponse == null) { Log("서버로부터 WA 데이터 수신 실패."); return; }
                displayLabel.Text = value;
                string gwResponse = await axis.SendCommandAsync("GW");
                Log(gwResponse != null ? "Received message: " + gwResponse : "서버로부터 GW 데이터 수신 실패.");
            }
            catch (Exception ex) { Log("오류 발생: " + ex.Message); }
        }

        // RR: WA 전송 후 RR 후속 명령 실행
        private async Task SendRrCommandAsync(AxisController axis, string value, Label displayLabel)
        {
            if (!axis.IsConnected) { Log("IP 주소를 선택하고 연결을 설정하세요."); return; }
            try
            {
                string waResponse = await axis.SendCommandAsync("WA" + value);
                if (waResponse == null) { Log("서버로부터 WA 데이터 수신 실패."); return; }
                displayLabel.Text = value;
                string rrResponse = await axis.SendCommandAsync("RR" + value);
                Log(rrResponse != null ? "Received message: " + rrResponse : "서버로부터 RR 데이터 수신 실패.");
            }
            catch (Exception ex) { Log("오류 발생: " + ex.Message); }
        }

        // 텔레메트리 폴링 루프 (UI 스레드에서 실행, 연결 해제 시 CancellationToken으로 중단)
        private async void StartAxisTelemetry(
            AxisController axis,
            Label posLabel, Label spLabel, Label acLabel,
            Label ovrdLabel, Label scrvLabel, Label slpnLabel, Label slppLabel)
        {
            var token = axis.TelemetryToken;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (axis.IsConnected)
                    {
                        TelemetryData data = await axis.PollTelemetryAsync();
                        if (data != null)
                        {
                            posLabel.Text  = data.Position;
                            spLabel.Text   = data.Speed;
                            acLabel.Text   = data.Accel;
                            ovrdLabel.Text = data.Override;
                            scrvLabel.Text = data.SCurve;
                            slpnLabel.Text = data.SlopeMinus;
                            slppLabel.Text = data.SlopePlus;
                        }
                    }
                }
                catch { }

                try { await Task.Delay(200, token); }
                catch (OperationCanceledException) { break; }
            }
        }


        // ─── 연결 / 해제 ─────────────────────────────────────────────────────────

        private void ConnectAxis(
            AxisController axis, ComboBox comboBox,
            Label posLabel, Label spLabel, Label acLabel,
            Label ovrdLabel, Label scrvLabel, Label slpnLabel, Label slppLabel)
        {
            if (comboBox.SelectedItem == null) { Log("IP 주소를 선택하세요."); return; }
            axis.Connect(comboBox.SelectedItem.ToString());
            Log("소켓 연결 성공");
            StartAxisTelemetry(axis, posLabel, spLabel, acLabel, ovrdLabel, scrvLabel, slpnLabel, slppLabel);
        }

        private void DisconnectAxis(AxisController axis, ComboBox comboBox)
        {
            if (comboBox.SelectedItem == null) { Log("IP 주소를 선택하세요."); return; }
            if (!axis.IsConnected) { Log("연결된 소켓이 없습니다."); return; }
            if (axis.Disconnect(comboBox.SelectedItem.ToString()))
                Log("소켓 연결이 해제되었습니다.");
            else
                Log("선택한 IP 주소와 현재 연결된 소켓의 IP 주소가 일치하지 않습니다.");
        }

        private void button1_Click(object sender, EventArgs e) =>
            ConnectAxis(axisX, comboBox1, label4, label25, label26, label27, label28, label65, label64);
        private void button2_Click(object sender, EventArgs e) =>
            DisconnectAxis(axisX, comboBox1);

        private void button3_Click(object sender, EventArgs e) =>
            ConnectAxis(axisY, comboBox2, label5, label32, label31, label30, label29, label69, label68);
        private void button4_Click(object sender, EventArgs e) =>
            DisconnectAxis(axisY, comboBox2);

        private void button5_Click(object sender, EventArgs e) =>
            ConnectAxis(axisZ, comboBox3, label6, label36, label35, label34, label33, label73, label72);
        private void button6_Click(object sender, EventArgs e) =>
            DisconnectAxis(axisZ, comboBox3);


        // ─── COMMAND (comboBox4 축 선택) ──────────────────────────────────────────

        private async void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            string command = textBox1.Text;

            if (comboBox4.SelectedItem == null) { Log("클라이언트를 선택하세요."); textBox1.Clear(); return; }
            AxisController axis = GetAxisFromComboBox4();
            if (axis == null) { Log("유효하지 않은 선택입니다."); textBox1.Clear(); return; }
            if (!axis.IsConnected) { Log("선택된 클라이언트가 없습니다."); textBox1.Clear(); return; }

            try
            {
                string response = await axis.SendCommandAsync(command == "REF" ? "REF" : command);
                Log(response != null ? "Received message: " + response : "서버로부터 데이터 수신 실패.");
            }
            catch (Exception ex) { Log("오류 발생: " + ex.Message); }
            textBox1.Clear();
        }


        // ─── REF ───────────────────────────────────────────────────────────────

        private async void button7_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisX, "ref");
        private async void button8_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisY, "ref");
        private async void button9_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisZ, "ref");

        // ─── GP0 ───────────────────────────────────────────────────────────────

        private async void button10_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisX, "g0");
        private async void button11_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisY, "g0");
        private async void button12_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisZ, "g0");

        // ─── SM ────────────────────────────────────────────────────────────────

        private async void button13_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisX, "SM");
        private async void button14_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisY, "SM");
        private async void button15_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisZ, "SM");

        // ─── PWC ───────────────────────────────────────────────────────────────

        private async void button16_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisX, "PWC");
        private async void button17_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisY, "PWC");
        private async void button18_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisZ, "PWC");

        // ─── PQ ────────────────────────────────────────────────────────────────

        private async void button19_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisX, "PQ");
        private async void button20_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisY, "PQ");
        private async void button21_Click(object sender, EventArgs e) => await SendSimpleCommandAsync(axisZ, "PQ");

        // ─── ABS ───────────────────────────────────────────────────────────────

        private async void textBox17_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "G", textBox17.Text, label43);
            textBox17.Clear();
        }
        private async void textBox21_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "G", textBox21.Text, label51);
            textBox21.Clear();
        }
        private async void textBox25_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "G", textBox25.Text, label59);
            textBox25.Clear();
        }

        // ─── REL ───────────────────────────────────────────────────────────────

        private async void textBox16_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendRelCommandAsync(axisX, textBox16.Text, label42);
            textBox16.Clear();
        }
        private async void textBox20_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendRelCommandAsync(axisY, textBox20.Text, label50);
            textBox20.Clear();
        }
        private async void textBox24_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendRelCommandAsync(axisZ, textBox24.Text, label58);
            textBox24.Clear();
        }

        // ─── RR ────────────────────────────────────────────────────────────────

        private async void textBox15_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendRrCommandAsync(axisX, textBox15.Text, label41);
            textBox15.Clear();
        }
        private async void textBox19_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendRrCommandAsync(axisY, textBox19.Text, label49);
            textBox19.Clear();
        }
        private async void textBox23_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendRrCommandAsync(axisZ, textBox23.Text, label57);
            textBox23.Clear();
        }

        // ─── WT ────────────────────────────────────────────────────────────────

        private async void textBox14_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "WT", textBox14.Text, label40);
            textBox14.Clear();
        }
        private async void textBox18_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "WT", textBox18.Text, label48);
            textBox18.Clear();
        }
        private async void textBox22_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "WT", textBox22.Text, label56);
            textBox22.Clear();
        }

        // ─── SP ────────────────────────────────────────────────────────────────

        private async void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "SP", textBox2.Text, label25);
            textBox2.Clear();
        }
        private async void textBox9_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "SP", textBox9.Text, label32);
            textBox9.Clear();
        }
        private async void textBox13_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "SP", textBox13.Text, label36);
            textBox13.Clear();
        }

        // ─── AC ────────────────────────────────────────────────────────────────

        private async void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "AC", textBox3.Text, label26);
            textBox3.Clear();
        }
        private async void textBox8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "AC", textBox8.Text, label31);
            textBox8.Clear();
        }
        private async void textBox12_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "AC", textBox12.Text, label35);
            textBox12.Clear();
        }

        // ─── OVRD ──────────────────────────────────────────────────────────────

        private async void textBox4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "OVRD", textBox4.Text, label27);
            textBox4.Clear();
        }
        private async void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "OVRD", textBox7.Text, label30);
            textBox7.Clear();
        }
        private async void textBox11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "OVRD", textBox11.Text, label34);
            textBox11.Clear();
        }

        // ─── S-Curve ───────────────────────────────────────────────────────────

        private async void textBox5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "SCRV", textBox5.Text, label28);
            textBox5.Clear();
        }
        private async void textBox6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "SCRV", textBox6.Text, label29);
            textBox6.Clear();
        }
        private async void textBox10_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "SCRV", textBox10.Text, label33);
            textBox10.Clear();
        }

        // ─── SLP- ──────────────────────────────────────────────────────────────

        private async void textBox27_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "SLPN", textBox27.Text, label65);
            textBox27.Clear();
        }
        private async void textBox29_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "SLPN", textBox29.Text, label69);
            textBox29.Clear();
        }
        private async void textBox31_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "SLPN", textBox31.Text, label73);
            textBox31.Clear();
        }

        // ─── SLP+ ──────────────────────────────────────────────────────────────

        private async void textBox26_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisX, "SLPP", textBox26.Text, label64);
            textBox26.Clear();
        }
        private async void textBox28_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisY, "SLPP", textBox28.Text, label68);
            textBox28.Clear();
        }
        private async void textBox30_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            await SendParamCommandAsync(axisZ, "SLPP", textBox30.Text, label72);
            textBox30.Clear();
        }

        private void label4_Click(object sender, EventArgs e) { }
    }
}
