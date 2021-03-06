﻿using DatingSite.Demo.Domain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace DatingSite.Demo
{
    public class DataAccess
    {
        private const string conString = "Server=(localdb)\\mssqllocaldb; Database=DatingSite";


        public List<Question> GetAllQuestions()
        {
            var sql = @"SELECT [Id], [Text]
                        FROM Question";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                var list = new List<Question>();

                while (reader.Read())
                {
                    var Question = new Question
                    {
                        Id = reader.GetSqlInt32(0).Value,
                        Text = reader.GetSqlString(1).Value,
                    };
                    list.Add(Question);
                }
                return list;
            }
        }


        public List<Answer> GetAllAnswers()
        {
            var sql = @"SELECT [Id], [QuestionId], [Text], [Score]
                        FROM Answer";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                var list = new List<Answer>();

                while (reader.Read())
                {
                    var Answer = new Answer
                    {
                        Id = reader.GetSqlInt32(0).Value,
                        QuestionId = reader.GetSqlInt32(1).Value,
                        Text = reader.GetSqlString(2).Value,
                        Score = reader.GetSqlInt32(3).Value
                    };
                    list.Add(Answer);
                }

                return list;
            }
        }


        public List<Person> GetAllPersons()
        {
            var sql = @"SELECT [Id], [Name], [Age], [Gender], [Sexuality]
                        FROM Person";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))

            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                var list = new List<Person>();

                while (reader.Read())
                {
                    var person = new Person
                    {
                        Id = reader.GetSqlInt32(0).Value,
                        Name = reader.GetSqlString(1).Value,
                        Age = reader.GetSqlInt32(2).Value,
                        Gender = reader.GetSqlString(3).Value,
                        Sexuality = reader.GetSqlString(4).Value
                    };
                    list.Add(person);
                }

                return list;

            }
        }

        public void AddPerson(Person newPerson)
        {
            var sql = "INSERT INTO Person (Name, Age, Gender, Sexuality) VALUES (@Name, @Age, @Gender, @Sexuality)";

            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Name", newPerson.Name));
            parameterList.Add(new SqlParameter("Age", newPerson.Age));
            parameterList.Add(new SqlParameter("Gender", newPerson.Gender));
            parameterList.Add(new SqlParameter("Sexuality", newPerson.Sexuality));

            ExecuteSql(sql, parameterList);
        }

        public void DeletePerson(Person oldPerson)
        {
            var sql = "DELETE FROM Person WHERE Id=@Id ";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Id", oldPerson.Id));

            ExecuteSql(sql, parameterList);
        }
        public void DeleteQuestion(Question oldQuestion)
        {
            var sql = "DELETE FROM Question WHERE Id=@Id";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Id", oldQuestion.Id));

            ExecuteSql(sql, parameterList);
        }
        public void DeleteAnswer(Question oldQuestion)
        {
            var sql = "DELETE FROM Answer WHERE QuestionId=@Id";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Id", oldQuestion.Id));

            ExecuteSql(sql, parameterList);
        }         

        public void ExecuteSql(string sql, List<SqlParameter> parameterList)
        {
            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                foreach (SqlParameter parameter in parameterList)
                {
                    command.Parameters.Add(parameter);
                }
                command.ExecuteNonQuery();
            }
        }

        internal void AddQuestion(Question newQuestion)
        {
            string sql = @"INSERT INTO Question (Text, Weight)
                        OUTPUT Inserted.Id
                        VALUES (@Text, @Weight)";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Text", newQuestion.Text));
            parameterList.Add(new SqlParameter("Weight", newQuestion.Weight));

            int createdId = ExecuteSqlAndReturnAffectedId(sql, parameterList);
            newQuestion.Id = createdId;
        }

        internal void AddAnswers(Question newQuestion)
        {
            foreach (Answer answer in newQuestion.Answers)
            {
                string sql = @"INSERT INTO Answer (QuestionId, Text, Score) 
                        VALUES (@QuestionId, @Text, @Score)";
                List<SqlParameter> parameterList = new List<SqlParameter>();
                parameterList.Add(new SqlParameter("QuestionId", newQuestion.Id));
                parameterList.Add(new SqlParameter("Text", answer.Text));
                parameterList.Add(new SqlParameter("Score", answer.Score));

                ExecuteSql(sql, parameterList);
            }
        }

        internal void AddPersonAnswers(int userId, List<PersonAnswerForQuestion> userAnswerList)
        {
            var sql = "INSERT INTO PersonAnswerForQuestion (PersonId, QuestionId, GivenAnswerId, Important, DesiredAnswerId) VALUES (@PersonId, @QuestionId, @GivenAnswerId, @Important, @DesiredAnswerId)";
            foreach (PersonAnswerForQuestion userAnswer in userAnswerList)
            {
                List<SqlParameter> parameterList = new List<SqlParameter>();
                parameterList.Add(new SqlParameter("PersonId", userId));
                parameterList.Add(new SqlParameter("QuestionId", userAnswer.QuestionId));
                parameterList.Add(new SqlParameter("GivenAnswerId", userAnswer.GivenAnswerId));
                parameterList.Add(new SqlParameter("Important", userAnswer.Important));
                parameterList.Add(new SqlParameter("DesiredAnswerId", userAnswer.DesiredAnswerId));
                ExecuteSql(sql, parameterList);
            }
        }

    public int ExecuteSqlAndReturnAffectedId(string sql, List<SqlParameter> parameterList)
        {
            int output;
            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();
                foreach (SqlParameter parameter in parameterList)
                {
                    command.Parameters.Add(parameter);
                }
                output = (int)command.ExecuteScalar();
            }
            return output;
        }
        public List<Person> GetAllPersonsWithAnswers()
        {

            List<Person> personList = GetAllPersons();

            GetAllPersonAnswers(personList);

            return personList;
        }

        private void GetAllPersonAnswers(List<Person> personList)
        {
            var sql = @"SELECT PersonId, PersonAnswerForQuestion.QuestionId, Question.Weight, GivenAnswer.Score, DesiredAnswer.Score, Important
                        FROM PersonAnswerForQuestion
                        LEFT JOIN Answer AS GivenAnswer ON GivenAnswerId = GivenAnswer.Id
                        LEFT JOIN Answer AS DesiredAnswer ON DesiredAnswerId = DesiredAnswer.Id
                        LEFT JOIN Question ON GivenAnswer.QuestionId = Question.Id";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int personId = reader.GetSqlInt32(0).Value;
                    Person currentPerson = personList.Where(x => x.Id == personId).First();
                    AddAnswerToPerson(currentPerson, reader);
                }
            }
        }

        
        private void AddAnswerToPerson(Person currentPerson, SqlDataReader reader)
        {
            //PersonAnswerForQuestion.QuestionId, Question.Weight, GivenAnswer.Score, DesiredAnswer.Score, Important
            PersonAnswerForQuestion answer = new PersonAnswerForQuestion
            {
                QuestionId = reader.GetSqlInt32(1).Value,
                QuestionWeight = reader.GetSqlInt32(2).Value,
                GivenAnswerScore = reader.GetSqlInt32(3).Value,
                DesiredAnswerScore = reader.GetSqlInt32(4).Value,
                Important = reader.GetSqlDouble(5).Value
            };
            currentPerson.PersonAnswers.Add(answer);

        }
        
    }
}