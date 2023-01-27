using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using static ClosedXML.Excel.XLPredefinedFormat;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace ValidadordeCPFdoRicardo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string dbCPF;
        bool validoOuNaoDb;


        private void button1_Click(object sender, EventArgs e)
        {
            //testa se o campo está vazio no click do botao

            if (maskValor.Text == "")
                MessageBox.Show("Insira um valor");
            else
            {
                //recebe o numero da maskedValor e faz limpeza do input
                string numero = maskValor.Text;
                numero = numero.Trim();
                numero = numero.Replace(".", "").Replace("-", "").Replace(",", "");

                //aqui manda para a classe CPF
                bool validoOuNao = (CPF.Validar(numero));


                if (validoOuNao == true)
                    MessageBox.Show("O CPF informado é válido");
                else
                    MessageBox.Show("O CPF informado não é válido.");


                // MessageBox.Show(CPF.Validar(numero).ToString());

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            maskValor.Text = "";

        }

        private void maskValor_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            MessageBox.Show("Insira somente números. \nO CPF deve ter 11 dígitos.");
        }



        // botao de ler o banco de dados
        private void button5_Click(object sender, EventArgs e)
        {

            toolStripStatusLabel1.Text = "Conectando...";
            statusStrip1.Refresh();

            try
            {
                using SqlConnection cn = new SqlConnection(Conn.StrCon);
                cn.Open();
                var sqlQuery = "SELECT * FROM dbo.Persons2";
                using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, cn))
                {
                    using (DataTable dt = new DataTable())
                    {
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;
                        dataGridView1.Columns[0].Width = 60;
                        dataGridView1.Columns[3].Width = 40;
                        //  dataGridView1.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dataGridView1.Columns[5].Width = 86;
                        dataGridView1.Columns[6].Width = 200;
                        dataGridView1.Columns[7].Width = 50;


                    }

                }

                toolStripStatusLabel1.Text = "Conectado ao banco de dados";
                statusStrip1.Refresh();
            }


            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Falha.";
                statusStrip1.Refresh();
                MessageBox.Show("Falha ao conectar\n\n" + ex.Message);

            }

        }
        //botao de conferir o cpf do banco de dados
        private void button3_Click_1(object sender, EventArgs e)
        {



            // Pegar o numero IDPessoa
            string numeroID = maskedTextBox1.Text;
            numeroID = numeroID.Trim();
            numeroID = numeroID.Replace(".", "").Replace("-", "").Replace(",", "").Replace("_", "");

            // Validar o numero IDPessoa
            if (string.IsNullOrWhiteSpace(numeroID))
            {
                MessageBox.Show("Insira um numero valido.");
                return;
            }

            // Conectar ao DB

            try
            {

                using SqlConnection cn = new SqlConnection(Conn.StrCon);
                cn.Open();

                // Pegar o CPF do IDPessoa
                string sqlQuery = "SELECT CPF from dbo.Persons2 WHERE IDPessoa = " + numeroID;
                using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, cn))
                {
                    using (DataTable dt = new DataTable())
                    {
                        da.Fill(dt);
                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("O registro informado não existe.");
                            return;
                        }

                        // Pegar o CPF do DB
                        dbCPF = dt.Rows[0]["CPF"].ToString();
                        dbCPF = dbCPF.Replace(".", "").Replace("-", "").Replace(",", "").Replace("_", "");

                        // Mandar o CPF para a Classe CPF e pegar o resultado validoounaodb

                        validoOuNaoDb = CPF.Validar(dbCPF);

                        if (validoOuNaoDb == true)
                            MessageBox.Show("O CPF do arquivo é Válido.");
                        else
                            MessageBox.Show("O CPF do arquivo é Inválido");

                        // Fazer update da coluna Válido no DB
                        string updateQuery = "UPDATE dbo.Persons2 SET Valido = @Valido WHERE IDPessoa = " + numeroID + "SELECT * FROM dbo.Persons2";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, cn))
                        {
                            cmd.Parameters.AddWithValue("@valido", validoOuNaoDb);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Falha.";
                statusStrip1.Refresh();
                MessageBox.Show("Falha ao conectar\n\n" + ex.Message);
            }

            finally
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                maskedTextBox2.Text = "";

                using SqlConnection cn = new SqlConnection(Conn.StrCon);
                cn.Open();
                var sqlQuery = "SELECT IDPessoa, Nome, Sobrenome, Idade, Cidade, CPF, Observacoes, Valido FROM dbo.Persons2";
                using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, cn))
                {
                    using (DataTable dt = new DataTable())
                    {
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                    }

                    cn.Close();




                }

            }

        }
        //botao de limpar 2
        private void button4_Click_1(object sender, EventArgs e)
        {
            maskedTextBox1.Text = "";
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        // botao de inserir
        private void button6_Click(object sender, EventArgs e)
        {
            int idVerificar;

            if (!int.TryParse(textBox6.Text, out idVerificar))
            {
                MessageBox.Show("ID Inválido, use um numero.");
                return;
            }

            else
            {


                try
                {
                    using SqlConnection cn = new SqlConnection(Conn.StrCon);
                    cn.Open();

                    // conferir se o ID ja existe

                    SqlCommand cmdConferir = new SqlCommand("SELECT COUNT(*) FROM dbo.Persons2 WHERE IDPessoa = @IDPessoa", cn);
                    cmdConferir.Parameters.AddWithValue("@IDPessoa", int.Parse(textBox6.Text));
                    int count = (int)cmdConferir.ExecuteScalar();

                    if (count > 0)
                    {
                        MessageBox.Show("o ID informado já existe, favor escolher outro.");
                    }
                    else
                    {
                        // inserir novos dados

                        SqlCommand cmd = new SqlCommand("insert into dbo.Persons2 values (@IDPessoa, @Nome, @Sobrenome, @Idade, @Cidade, @CPF, @Observacoes, @Valido)", cn);

                        cmd.Parameters.AddWithValue("@IDPessoa", int.Parse(textBox6.Text));
                        cmd.Parameters.AddWithValue("@Nome", textBox1.Text);
                        cmd.Parameters.AddWithValue("@Sobrenome", textBox2.Text);
                        cmd.Parameters.AddWithValue("@Idade", int.Parse(textBox5.Text));
                        cmd.Parameters.AddWithValue("@Cidade", textBox3.Text);
                        cmd.Parameters.AddWithValue("@CPF", maskedTextBox2.Text);
                        cmd.Parameters.AddWithValue("@Observacoes", textBox4.Text);
                        cmd.Parameters.AddWithValue("@Valido", 0);


                        cmd.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Inserido com Sucesso");
                    }
                }




                catch (SqlException sqlEx)
                {
                    MessageBox.Show("Erro ao conectar DB: " + sqlEx.Message);

                }

                catch (Exception ex)
                {

                    MessageBox.Show("Erro: " + ex.Message);


                }

                finally
                {
                    textBox1.Text = "";
                    textBox2.Text = "";
                    textBox3.Text = "";
                    textBox4.Text = "";
                    textBox5.Text = "";
                    textBox6.Text = "";
                    maskedTextBox2.Text = "";

                    using SqlConnection cn = new SqlConnection(Conn.StrCon);
                    cn.Open();
                    var sqlQuery = "SELECT IDPessoa, Nome, Sobrenome, Idade, Cidade, CPF, Observacoes, Valido FROM dbo.Persons2";
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, cn))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            da.Fill(dt);
                            dataGridView1.DataSource = dt;

                        }

                        cn.Close();




                    }
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            try
            {
                using SqlConnection cn = new SqlConnection(Conn.StrCon);
                cn.Open();
                SqlCommand cmd = new SqlCommand("update dbo.Persons2 set Nome=@Nome, Sobrenome=@Sobrenome, Idade=@Idade, Cidade=@Cidade, CPF=@CPF, Observacoes=@Observacoes where IDPessoa=@IDPessoa", cn);


                cmd.Parameters.AddWithValue("@IDPessoa", int.Parse(textBox6.Text));
                cmd.Parameters.AddWithValue("@Nome", textBox1.Text);
                cmd.Parameters.AddWithValue("@Sobrenome", textBox2.Text);
                cmd.Parameters.AddWithValue("@Idade", int.Parse(textBox5.Text));
                cmd.Parameters.AddWithValue("@Cidade", textBox3.Text);
                cmd.Parameters.AddWithValue("@CPF", maskedTextBox2.Text);
                cmd.Parameters.AddWithValue("@Observacoes", textBox4.Text);
                cmd.Parameters.AddWithValue("@Valido", 0);
                cmd.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("Atualizado com Sucesso");
            }

            catch (SqlException sqlEx)
            {
                MessageBox.Show("Erro ao conectar ao banco de dados: " + sqlEx.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);

            }

            finally
            {
                textBox1.Text = "";
                textBox2.Text = "";
                textBox3.Text = "";
                textBox4.Text = "";
                textBox5.Text = "";
                textBox6.Text = "";
                maskedTextBox2.Text = "";

                using SqlConnection cn = new SqlConnection(Conn.StrCon);
                cn.Open();
                var sqlQuery = "SELECT IDPessoa, Nome, Sobrenome, Idade, Cidade, CPF, Observacoes, Valido FROM dbo.Persons2";
                using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, cn))
                {
                    using (DataTable dt = new DataTable())
                    {
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                    }

                    cn.Close();




                }


            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            maskedTextBox2.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                using SqlConnection cn = new SqlConnection(Conn.StrCon);
                cn.Open();
                SqlCommand cmd = new SqlCommand("delete from dbo.Persons2 where IDPessoa=@IDPessoa", cn);
                cmd.Parameters.AddWithValue("@IDPessoa", int.Parse(textBox6.Text));
                cmd.ExecuteNonQuery();
                cn.Close();
                MessageBox.Show("Deletado com Sucesso");
            }

            catch (SqlException sqlEx)
            {
                MessageBox.Show("Erro ao conectar ao banco de dados: " + sqlEx.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show("Erro: " + ex.Message);

            }
            finally
            {
                using SqlConnection cn = new SqlConnection(Conn.StrCon);
                cn.Open();
                var sqlQuery = "SELECT IDPessoa, Nome, Sobrenome, Idade, Cidade, CPF, Observacoes, Valido FROM dbo.Persons2";
                using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, cn))
                {
                    using (DataTable dt = new DataTable())
                    {
                        da.Fill(dt);
                        dataGridView1.DataSource = dt;

                    }

                    cn.Close();


                }





            }
        }






    }


}


