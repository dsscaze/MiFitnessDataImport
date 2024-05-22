using Microsoft.VisualBasic.FileIO;
using System;
using System.Windows.Forms;

namespace MiFitnessDataImportAgent
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btImportSportData_Click(object sender, EventArgs e)
        {
            lerSportCSV();
        }

        private void lerSportCSV()
        {
            AcessoDados _bd = new AcessoDados();
            string caminhoBase = @"C:\Users\dssca\Downloads\xiaomi-data-mifitness-20240521\20240522_6599729986_MiFitness_c3_data_copy-f1\";
            string arquivoSport = @"20240522_6599729986_MiFitness_hlth_center_sport_record.csv";
            string arquivoFitness = @"20240522_6599729986_MiFitness_hlth_center_fitness_data.csv";

            using (TextFieldParser parser = new TextFieldParser(caminhoBase + arquivoSport))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                int lineNumber = 1;

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (lineNumber > 1)
                    {
                        string uid = fields[0];
                        string sid = fields[1];
                        string key = fields[2];
                        string time = fields[3];
                        string category = fields[4];
                        string value = fields[5];
                        string updatetime = fields[6];

                        string sqlInsertSport = @"insert into sportrecord (Uid,Sid,[Key],Time,Category,Value,UpdateTime) ";
                        sqlInsertSport += string.Format("values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                            uid, sid, key, time, category, value, updatetime);

                        _bd.executaComando(sqlInsertSport);
                    }

                    lineNumber++;
                }
            }

            using (TextFieldParser parser = new TextFieldParser(caminhoBase + arquivoFitness))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(",");

                int lineNumber = 1;

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();
                    if (lineNumber > 1)
                    {
                        string uid = fields[0];
                        string sid = fields[1];
                        string key = fields[2];
                        string time = fields[3];
                        string value = fields[4];
                        string updatetime = fields[5];

                        string sqlInsertSport = @"insert into fitnessdata (Uid,Sid,[Key],Time,Value,UpdateTime) ";
                        sqlInsertSport += string.Format("values ('{0}','{1}','{2}','{3}','{4}','{5}')",
                            uid, sid, key, time, value, updatetime);

                        _bd.executaComando(sqlInsertSport);
                    }

                    lineNumber++;
                }
            }
        }
    }
}