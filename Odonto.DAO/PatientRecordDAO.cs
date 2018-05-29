using Dapper;
using Npgsql;
using Odonto.Models;
using System;
using System.Collections.Generic;

namespace Odonto.DAO
{
    public class PatientRecordDAO
    {
        private string strConnection;

        public PatientRecordDAO(string strConn)
        {
            strConnection = strConn;
        }

        public List<PatientRecord> GetAll()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<PatientRecord>("SELECT * FROM PatientsRecord").AsList();
                return list;
            }
        }

        public List<PatientRecord> GetByPage(int Page, int Size)
        {
            int IndexStart = (Page * Size) - Size;
            int IndexEnd = Page * Size;
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<PatientRecord>("SELECT * FROM (SELECT Row_Number() OVER(ORDER BY ID) AS RowIndex, * FROM PatientsRecord) As PatientsRecord WHERE PatientsRecord.RowIndex > " + IndexStart + " AND PatientsRecord.RowIndex <= " + IndexEnd).AsList();
                return list;
            }
        }

        public PatientRecord GetById(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<PatientRecord>("SELECT * FROM PatientsRecord WHERE PatientID = @ID", new { ID = ID });
            }
        }

        public List<PatientRecordDisease> GetDiseases(int patientId)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<PatientRecordDisease>(@"SELECT Record.ID, Record.PatientRecordID, Record.DiseaseID, Record.Description, Diseases.Name As DiseaseLabel FROM PatientsRecordDisease As Record 
                                                            LEFT JOIN Diseases ON Record.DiseaseID = Diseases.ID WHERE PatientRecordID = @PatientRecordID", 
                                                            new { PatientRecordID = patientId }).AsList();
                return list;
            }
        }

        public List<PatientRecordProcedure> GetProcedures(int patientId)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var list = sql.Query<PatientRecordProcedure>(@"SELECT Persons.Name As DentistName, Record.ID, Record.Description, PatientRecordID, DentistID, ProcedureID, Date, Record.Value, Procedures.Name As ProcedureLabel
                                                                FROM PatientsRecordProcedure As Record LEFT JOIN Procedures ON Record.ProcedureID = Procedures.ID
                                                                LEFT JOIN Persons ON Record.DentistID = Persons.ID
                                                                WHERE PatientRecordID = @PatientRecordID
                                                                ORDER BY Date DESC", 
                                                                new { PatientRecordID = patientId }).AsList();
                return list;
            }
        }
        
        public bool Add(PatientRecord PatientRecord)
        {
            PatientRecord.CreatedOn = DateTime.Now;
            PatientRecord.UpdatedOn = DateTime.Now;

            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"INSERT INTO PatientsRecord (PatientID,MainComplaint,Alterations,Medicines,PsychologicalTreatment,Diagnose,CreatedOn,UpdatedOn,CreatedBy,UpdatedBy)
                                        VALUES (@PatientID,@MainComplaint,@Alterations,@Medicines,@PsychologicalTreatment,@Diagnose,@CreatedOn,@UpdatedOn,@CreatedBy,@UpdatedBy)",
                                        new
                                        {
                                            PatientID = PatientRecord.PatientID,
                                            MainComplaint = PatientRecord.MainComplaint,
                                            Alterations = PatientRecord.Alterations,
                                            Medicines = PatientRecord.Medicines,
                                            PsychologicalTreatment = PatientRecord.PsychologicalTreatment,
                                            Diagnose = PatientRecord.Diagnose,
                                            CreatedOn = PatientRecord.CreatedOn,
                                            UpdatedOn = PatientRecord.UpdatedOn,
                                            CreatedBy = PatientRecord.CreatedBy,
                                            UpdatedBy = PatientRecord.UpdatedBy
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool AddDiseases(List<PatientRecordDisease> PatientDiseases)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                foreach(var item in PatientDiseases)
                {
                    sql.Execute(@"INSERT INTO PatientsRecordDisease (PatientRecordID,DiseaseID,Description)
                                VALUES (@PatientRecordID,@DiseaseID,@Description)",
                                new
                                {
                                    PatientRecordID = item.PatientRecordID,
                                    DiseaseID = item.DiseaseID,
                                    Description = item.Description,
                                });
                }
                return true; 
            }
        }

        public bool AddProcedure(PatientRecordProcedure PatientProcedure)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                sql.Execute(@"INSERT INTO PatientsRecordProcedure (PatientRecordID,ProcedureID,DentistID,Description,Value,Date)
                            VALUES (@PatientRecordID,@ProcedureID,@DentistID,@Description,@Value,@Date)",
                            new
                            {
                                PatientRecordID = PatientProcedure.PatientRecordID,
                                ProcedureID = PatientProcedure.ProcedureID,
                                DentistID = PatientProcedure.DentistID,
                                Description = PatientProcedure.Description,
                                Value = PatientProcedure.Value,
                                Date = PatientProcedure.Date,
                            });
                return true;
            }
        }

        public bool Edit(PatientRecord PatientRecord)
        {
            PatientRecord.UpdatedOn = DateTime.Now;

            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute(@"UPDATE PatientsRecord SET MainComplaint = @MainComplaint,Alterations = @Alterations,Medicines = @Medicines,PsychologicalTreatment = @PsychologicalTreatment,Diagnose = @Diagnose,UpdatedOn = @UpdatedOn,UpdatedBy = @UpdatedBy
                                            WHERE PatientID=@PatientID",
                                        new
                                        {
                                            PatientID = PatientRecord.PatientID,
                                            MainComplaint = PatientRecord.MainComplaint,
                                            Alterations = PatientRecord.Alterations,
                                            Medicines = PatientRecord.Medicines,
                                            PsychologicalTreatment = PatientRecord.PsychologicalTreatment,
                                            Diagnose = PatientRecord.Diagnose,
                                            UpdatedOn = PatientRecord.UpdatedOn,
                                            UpdatedBy = PatientRecord.UpdatedBy
                                        });
                return Convert.ToBoolean(resp);
            }
        }

        public bool Remove(int ID)
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                var resp = sql.Execute("DELETE FROM PatientsRecord WHERE PatientID = @ID", new { ID });

                return Convert.ToBoolean(resp);
            }
        }

        public int Length()
        {
            using (var sql = new NpgsqlConnection(strConnection))
            {
                return sql.QueryFirstOrDefault<int>("SELECT COUNT(1) FROM PatientsRecord");
            }
        }
    }
}