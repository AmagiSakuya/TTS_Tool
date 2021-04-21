using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;

namespace TTS_Tool
{

    public partial class Form1 : Form
    {
        string settingsPath = "./Settings.json"; //配置文件路径
        string dicPath = "./Dictionary.json"; //字典路径

        SpeechSynthesizer m_speaker;
        Settings m_settings;
        WordConvertionItems m_dic;

        // 检查配置文件是否存在
        bool CheckSettings()
        {
            if (!File.Exists(settingsPath)) return false;
            if (!File.Exists(dicPath)) return false;
            return true;
        }

        // 读取配置文件
        bool LoadSettings(out Settings settings, out WordConvertionItems dic)
        {
            string settingsStr = File.ReadAllText(settingsPath);
            string dicStr = File.ReadAllText(dicPath);
            try
            {
                settings = JsonConvert.DeserializeObject<Settings>(settingsStr);
                dic = JsonConvert.DeserializeObject<WordConvertionItems>(dicStr);
                return true;
            }
            catch (Exception e)
            {
                settings = null;
                dic = null;
                return false;
            }
        }

        bool CheckVoicer(SpeechSynthesizer speaker, string voicerName)
        {
            var voices = speaker.GetInstalledVoices();
            for (int i = 0; i < voices.Count; i++)
            {
                if (voices[i].VoiceInfo.Name.Equals(voicerName))
                {
                    return true;
                }
            }
            return false;
        }

        bool CheckSpeakReady(out SpeechSynthesizer speaker, out Settings settings, out WordConvertionItems dic, out string errorMsg)
        {
            speaker = new SpeechSynthesizer();
            errorMsg = "";
            settings = null;
            dic = null;
            if (!CheckSettings())
            {
                errorMsg = "读取配置文件错误";
                return false;
            }

            if (!LoadSettings(out settings, out dic))
            {
                errorMsg = "读取配置文件错误";
                return false;
            }

            if (!CheckVoicer(speaker, settings.voicerName))
            {
                errorMsg = $"未能获取到语音者{settings.voicerName}";
                return false;
            }
            return true;
        }

        bool InitSpeaker(out SpeechSynthesizer speaker, out Settings settings, out WordConvertionItems dic, out string errorMsg)
        {
            if (CheckSpeakReady(out speaker, out settings, out dic, out errorMsg))
            {
                speaker.SelectVoice(settings.voicerName);
                if (settings.SpeakRate > 10) settings.SpeakRate = 10;
                if (settings.SpeakRate < -10) settings.SpeakRate = -10;
                speaker.Rate = settings.SpeakRate;
                speaker.Volume = settings.speakVolume;
                return true;
            }
            return false;
        }

        string ReplaceWorldWithDic(string msg)
        {
            string result = msg;
            if (m_dic != null && !string.IsNullOrEmpty(msg))
            {
                for (int i = 0; i < m_dic.dictionary.Length; i++)
                {
                    result = msg.Replace(m_dic.dictionary[i].origin, m_dic.dictionary[i].output);
                }
            }
            return result;
        }

        string[] ReplaceWorldWithDic(string[] msgs)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < msgs.Length; i++)
            {
                result.Add(ReplaceWorldWithDic(msgs[i]));
            }
            return result.ToArray();
        }

        public Form1()
        {
            InitializeComponent();
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length != 3 || !(args[1].Equals("-speak"))) return;

            //enter cmd mode
            string errorMsg;
            if (InitSpeaker(out m_speaker, out m_settings, out m_dic, out errorMsg))
            {
                m_speaker.Speak(ReplaceWorldWithDic(args[2]));
                m_speaker.Dispose();
            }
            else
            {
                Console.WriteLine(errorMsg);
            }
            Process.GetCurrentProcess().Kill();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        //点击【开始】
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            string errorMsg;
            if (!InitSpeaker(out m_speaker, out m_settings, out m_dic, out errorMsg))
            {
                MessageBox.Show(errorMsg, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            OpenFileDialog file = new OpenFileDialog();
            file.Filter = "文本文档|*.txt";
            file.ShowDialog();

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择保存路径";
            dialog.ShowDialog();
            Console.WriteLine(dialog.SelectedPath);

            string finalPath = $"{dialog.SelectedPath}/Output";
            if (!Directory.Exists(finalPath))//如果不存在就创建file文件夹
            {
                Directory.CreateDirectory(finalPath);
            }

            string m_text = File.ReadAllText(file.FileName);
            string[] m_texts = m_text.Split(Environment.NewLine.ToCharArray());

            ReplaceWorldWithDic(m_texts);
            if (m_dic != null)
            {
                for (int i = 0; i < m_texts.Length; i++)
                {
                    if (string.IsNullOrEmpty(m_texts[i])) continue;
                    m_speaker.SetOutputToWaveFile($"{finalPath}/{m_texts[i]}.wav");
                    m_texts[i] = ReplaceWorldWithDic(m_texts[i]);
                    m_speaker.Speak(m_texts[i]);
                }
            }
            m_speaker.SetOutputToNull();
            m_speaker.Dispose();
            MessageBox.Show("成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Process.Start(dialog.SelectedPath);
        }

    }
    public class Settings
    {
        public string voicerName;
        public int speakVolume;
        public int SpeakRate;
    }
    public class WordConvertionItem
    {
        public string origin;
        public string output;
    }
    public class WordConvertionItems
    {
        public WordConvertionItem[] dictionary;
    }
}
