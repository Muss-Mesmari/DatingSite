﻿using System;
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
                        Text = reader.GetSqlString(2).Value,            
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

<<<<<<< HEAD
        internal void AddQuestion(Question newQuestion)
        {
            string sql = @"INSERT INTO Question (Text)
                        OUTPUT Inserted.Id
                        VALUES (@Text)";
            List<SqlParameter> parameterList = new List<SqlParameter>();
            parameterList.Add(new SqlParameter("Text", newQuestion.Text));

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

=======
>>>>>>> 38abff01522da96c1423a33bcd1f0b4c48ee0084
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
            var sql = @"SELECT Person.Id, [Name], [Age], [Gender], [Sexuality], [QuestionId], [Score], [Important]
                        FROM UserAnswerForQuestion LEFT JOIN Person ON UserId = Person.Id
                        LEFT JOIN Answer ON AnswerId = Answer.Id";

            using (SqlConnection connection = new SqlConnection(conString))
            using (SqlCommand command = new SqlCommand(sql, connection))
            {
                connection.Open();

                SqlDataReader reader = command.ExecuteReader();

                var list = new List<Person>();

                while (reader.Read())
                {
                    CreatePersonInList(ref list, reader);

                    AddAnswerToPerson(ref list, reader);
                }
                return list;
            }
        }
        private void CreatePersonInList(ref List<Person> personList, SqlDataReader reader)
        {
            int id = reader.GetSqlInt32(0).Value;

            if (!personList.Select(x => x.Id).Contains(id))
            {
                var person = new Person
                {
                    Id = reader.GetSqlInt32(0).Value,
                    Name = reader.GetSqlString(1).Value,
                    Age = reader.GetSqlInt32(2).Value,
                    Gender = reader.GetSqlString(3).Value,
                    Sexuality = reader.GetSqlString(4).Value
                };
                personList.Add(person);
            }
        }
        private void AddAnswerToPerson(ref List<Person> personList, SqlDataReader reader)
        {
            Answer answer = new Answer {
                QuestionId = reader.GetSqlInt32(5).Value,
                Score = reader.GetSqlInt32(6).Value,
                Important = reader.GetSqlBoolean(7).Value
            };
            int id = reader.GetSqlInt32(0).Value;
            personList.Where(x => x.Id == id).First().AnswerList.Add(answer);
        }

        //public List<BlogPost> GetAllBlogPostsBrief()
        //{
        //    var sql = @"SELECT [Id], [Author], [Title]
        //                FROM BlogPost";

        //    using (SqlConnection connection = new SqlConnection(conString))
        //    using (SqlCommand command = new SqlCommand(sql, connection))
        //    {
        //        connection.Open();

        //        SqlDataReader reader = command.ExecuteReader();

        //        var list = new List<BlogPost>();

        //        while (reader.Read())
        //        {
        //            var bp = new BlogPost
        //            {
        //                Id = reader.GetSqlInt32(0).Value,
        //                Author = reader.GetSqlString(1).Value,
        //                Title = reader.GetSqlString(2).Value
        //            };
        //            list.Add(bp);
        //        }

        //        return list;

        //    }
        //}

        //public BlogPost GetPostById(int postId)
        //{
        //    var sql = @"SELECT [Id], [Author], [Title]
        //                FROM BlogPost 
        //                WHERE Id=@Id";

        //    using (SqlConnection connection = new SqlConnection(conString))
        //    using (SqlCommand command = new SqlCommand(sql, connection))
        //    {
        //        connection.Open();
        //        command.Parameters.Add(new SqlParameter("Id", postId));

        //        SqlDataReader reader = command.ExecuteReader();

        //        if (reader.Read())
        //        {
        //            var bp = new BlogPost
        //            {
        //                Id = reader.GetSqlInt32(0).Value,
        //                Author = reader.GetSqlString(1).Value,
        //                Title = reader.GetSqlString(2).Value
        //            };
        //            return bp;

        //        }

        //        return null;

        //    }
        //}



        //public void UpdateBlogpost(int id, string title)
        //{
        //    var sql = "UPDATE BlogPost SET Title=@Title WHERE Id=@id";

        //    using (SqlConnection connection = new SqlConnection(conString))
        //    using (SqlCommand command = new SqlCommand(sql, connection))
        //    {
        //        connection.Open();
        //        command.Parameters.Add(new SqlParameter("Id", id));
        //        command.Parameters.Add(new SqlParameter("Title", title));
        //        command.ExecuteNonQuery();
        //    }
        //}


        //public void AddTag(string tag)
        //{
        //    var sql = "INSERT INTO TagPost (Tag) VALUES (@tag)";

        //    using (SqlConnection connection = new SqlConnection(conString))
        //    using (SqlCommand command = new SqlCommand(sql, connection))
        //    {
        //        connection.Open();
        //        command.Parameters.Add(new SqlParameter("Tag", tag));
        //        command.ExecuteNonQuery();
        //    }
        //}

    }
}