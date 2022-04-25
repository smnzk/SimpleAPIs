using PrzykladKolokwium.Models;
using System.Data.SqlClient;

namespace PrzykladKolokwium.Services
{
    public class DatabaseOperator : IDatabaseOperator
    {

        private string connectionString = "Data Source=localhost;Initial Catalog=APBD;Integrated Security=True";

        public async Task<Information> PostMedicaments(IList<Medicament> medicaments, int idPrescription)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                await con.OpenAsync();da

                var tran = await con.BeginTransactionAsync();

                try
                {
                    cmd.Transaction = (SqlTransaction) tran;

                    foreach (Medicament medicament in medicaments)
                    {
                        bool medicamentExists = await CheckIfMedicamentExists(cmd, medicament.idMedicament);
                        if (medicamentExists == false)
                        {
                            return new Information(false, "Nie ma takiego leku");
                        }

                        bool medicmanetAlreadyPrescribed = 
                    }


                } catch (Exception ex)
                {
                    await tran.RollbackAsync(); 
                }

            }
               
        }


        public async Task<bool> CheckIfMedicamentExists(SqlCommand cmd, int id)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT 1 FROM Medicament WHERE IdMedicament = @id";
            cmd.Parameters.AddWithValue("@id",id);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                await reader.CloseAsync();
                return true;
            }
            else
            {
                await reader.CloseAsync();
                return false;
            }

        }


        public async Task<bool> CheckIfMedicamentAlreadyPrescribed(SqlCommand cmd, int prescriptionId, int medicamentId)
        {
            cmd.Parameters.Clear();
            cmd.CommandText = "SELECT 1 FROM Prescription_Medicament WHERE IdPrescription = @idpr AND IdMedicament = @idmed";
            cmd.Parameters.AddWithValue("@idpr", prescriptionId);
            cmd.Parameters.AddWithValue("@idmed", medicamentId);

            SqlDataReader reader = await cmd.ExecuteReaderAsync();

            if(await reader.ReadAsync())
            {
                await reader.CloseAsync();
                return true;
            } else
            {
                await reader.CloseAsync();
                return false;
            }
        }

        public async Task<IList<Prescription>> GetPrescriptions()
        {
            IList<Prescription> prescriptions = new List<Prescription>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT * FROM Prescription ORDER BY Date DESC";

                await con.OpenAsync();

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    prescriptions.Add(new Prescription
                    {
                        IdPrescrption = int.Parse(reader["IdPrescription"].ToString()),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                        IdPatient = int.Parse(reader["IdPatient"].ToString()),
                        IdDoctor = int.Parse(reader["IdDoctor"].ToString())
                    });
                }
                await reader.CloseAsync();
                return prescriptions;
            }
        }

        public async Task<IList<Prescription>> GetPrescriptions(string nazwisko)
        {
            IList<Prescription> prescrptions = new List<Prescription>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "SELECT IdPatient FROM Patient WHERE LastName = @nazwisko";
                cmd.Parameters.AddWithValue("@nazwisko", nazwisko);

                await con.OpenAsync();

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                int amount = 0;
                int id = 0;
                while(await reader.ReadAsync())
                {
                    id = int.Parse(reader["IdPatient"].ToString());
                    amount++;
                }
                if(amount > 1)
                {
                    throw new HttpRequestException("Jest więcej niż jeden pacjent o tym nazwisku");
                }
                await reader.CloseAsync();

                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT * FROM Prescription WHERE IdPatient = @id ORDER BY Date DESC";
                cmd.Parameters.AddWithValue("@id", id);

                reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    prescrptions.Add(new Prescription
                    {
                        IdPrescrption = int.Parse(reader["IdPrescription"].ToString()),
                        Date = DateTime.Parse(reader["Date"].ToString()),
                        DueDate = DateTime.Parse(reader["DueDate"].ToString()),
                        IdPatient = int.Parse(reader["IdPatient"].ToString()),
                        IdDoctor = int.Parse(reader["IdDoctor"].ToString())
                    });
                }
                await reader.CloseAsync();
                return prescrptions;
            }
        }
    }
}
