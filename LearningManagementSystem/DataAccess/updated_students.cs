using LearningManagementSystem.Models;
using System;
using System.Collections.ObjectModel;
namespace LearningManagementSystem.DataAccess
{
    public class DataProvider
    {
        private static readonly Random _rng = new();
        static public ObservableCollection<StudentVer2> StudentList()
        {
            int rng = _rng.Next(0, 30);
            int deviation = (rng < 15) ? 0 : ((rng < 25 ? 1 : 2));
            foreach (var item in _studentList)
            {
                item.GraduationYear =
                    (item.EnrolledYear + 4 + deviation <= 2024) ?
                    item.EnrolledYear + 4 + deviation :
                    null;
            }
            return _studentList;
        }    

        private static readonly ObservableCollection<StudentVer2> _studentList = [
            new StudentVer2{
            Id = 1,
            StudentCode = "SC0001",
            StudentName = "Moana Rojas",
            Email = "nunc.mauris@yahoo.org",
            BirthDate = new DateTime(2024, 04, 24),
            PhoneNo = "0041-768-498",
            UserId = 1,
            EnrolledYear = 2022
        },
        new StudentVer2{
            Id = 2,
            StudentCode = "SC0002",
            StudentName = "Tanek Underwood",
            Email = "arcu.eu.odio@protonmail.couk",
            BirthDate = new DateTime(2024, 08, 27),
            PhoneNo = "0262-566-882",
            UserId = 2,
            EnrolledYear = 2023
        },
        new StudentVer2{
            Id = 3,
            StudentCode = "SC0003",
            StudentName = "Keefe Bolton",
            Email = "blandit.congue.in@protonmail.org",
            BirthDate = new DateTime(2024, 05, 01),
            PhoneNo = "0568-423-451",
            UserId = 3,
            EnrolledYear = 2020
        },
        new StudentVer2{
            Id = 4,
            StudentCode = "SC0004",
            StudentName = "Harrison Campos",
            Email = "tortor.at@google.edu",
            BirthDate = new DateTime(2024, 01, 16),
            PhoneNo = "0517-314-841",
            UserId = 4,
            EnrolledYear = 2017
        },
        new StudentVer2{
            Id = 5,
            StudentCode = "SC0005",
            StudentName = "Chanda Lucas",
            Email = "magnis.dis@hotmail.net",
            BirthDate = new DateTime(2024, 09, 09),
            PhoneNo = "0571-218-850",
            UserId = 5,
            EnrolledYear = 2024
        },
        new StudentVer2{
            Id = 6,
            StudentCode = "SC0006",
            StudentName = "Ryder Cochran",
            Email = "sit.amet@hotmail.edu",
            BirthDate = new DateTime(2024, 04, 03),
            PhoneNo = "0621-515-576",
            UserId = 6,
            EnrolledYear = 2024
        },
        new StudentVer2{
            Id = 7,
            StudentCode = "SC0007",
            StudentName = "Ethan Burt",
            Email = "nunc.mauris@outlook.org",
            BirthDate = new DateTime(2024, 09, 13),
            PhoneNo = "0212-717-340",
            UserId = 7,
            EnrolledYear = 2019
        },
        new StudentVer2{
            Id = 8,
            StudentCode = "SC0008",
            StudentName = "Martin Burch",
            Email = "aliquam.ultrices@yahoo.com",
            BirthDate = new DateTime(2025, 02, 15),
            PhoneNo = "0053-317-146",
            UserId = 8,
            EnrolledYear = 2019
        },
        new StudentVer2{
            Id = 9,
            StudentCode = "SC0009",
            StudentName = "Yasir Lawrence",
            Email = "purus.in@outlook.com",
            BirthDate = new DateTime(2024, 06, 15),
            PhoneNo = "0656-041-765",
            UserId = 9,
            EnrolledYear = 2021
        },
        new StudentVer2{
            Id = 10,
            StudentCode = "SC00010",
            StudentName = "Harrison Sloan",
            Email = "nibh.donec@outlook.com",
            BirthDate = new DateTime(2024, 04, 17),
            PhoneNo = "0711-889-247",
            UserId = 10,
            EnrolledYear = 2019
        },
        new StudentVer2{
            Id = 11,
            StudentCode = "SC00011",
            StudentName = "Jerry Holden",
            Email = "magna.cras.convallis@google.couk",
            BirthDate = new DateTime(2025, 08, 21),
            PhoneNo = "0887-250-384",
            UserId = 11,
            EnrolledYear = 2019
        },
        new StudentVer2{
            Id = 12,
            StudentCode = "SC00012",
            StudentName = "Chadwick Herrera",
            Email = "pellentesque.sed.dictum@aol.couk",
            BirthDate = new DateTime(2024, 07, 14),
            PhoneNo = "0358-423-564",
            UserId = 12,
            EnrolledYear = 2024
        },
        new StudentVer2{
            Id = 13,
            StudentCode = "SC00013",
            StudentName = "Lawrence Glass",
            Email = "feugiat.tellus@protonmail.net",
            BirthDate = new DateTime(2024, 06, 23),
            PhoneNo = "0662-624-517",
            UserId = 13,
            EnrolledYear = 2019
        },
        new StudentVer2{
            Id = 14,
            StudentCode = "SC00014",
            StudentName = "Hope Gay",
            Email = "nec@yahoo.net",
            BirthDate = new DateTime(2024, 05, 18),
            PhoneNo = "0787-355-779",
            UserId = 14,
            EnrolledYear = 2023
        },
        new StudentVer2{
            Id = 15,
            StudentCode = "SC00015",
            StudentName = "Brenna Wood",
            Email = "mi@yahoo.org",
            BirthDate = new DateTime(2024, 07, 04),
            PhoneNo = "0015-119-284",
            UserId = 15,
            EnrolledYear = 2020
        },
        new StudentVer2{
            Id = 16,
            StudentCode = "SC00016",
            StudentName = "Uriah Murphy",
            Email = "mollis.nec.cursus@google.edu",
            BirthDate = new DateTime(2024, 04, 07),
            PhoneNo = "0895-326-452",
            UserId = 16,
            EnrolledYear = 2023
        },
        new StudentVer2{
            Id = 17,
            StudentCode = "SC00017",
            StudentName = "Rafael Alvarez",
            Email = "dapibus.ligula@icloud.edu",
            BirthDate = new DateTime(2024, 05, 26),
            PhoneNo = "0148-565-158",
            UserId = 17,
            EnrolledYear = 2020
        },
        new StudentVer2{
            Id = 18,
            StudentCode = "SC00018",
            StudentName = "Hashim Thornton",
            Email = "non.sollicitudin.a@protonmail.org",
            BirthDate = new DateTime(2024, 10, 21),
            PhoneNo = "0648-127-362",
            UserId = 18,
            EnrolledYear = 2019
        },
        new StudentVer2{
            Id = 19,
            StudentCode = "SC00019",
            StudentName = "Deirdre Carr",
            Email = "egestas.urna@google.edu",
            BirthDate = new DateTime(2025, 09, 12),
            PhoneNo = "0568-718-660",
            UserId = 19,
            EnrolledYear = 2023
        },
        new StudentVer2{
            Id = 20,
            StudentCode = "SC00020",
            StudentName = "Nell Baldwin",
            Email = "sem.elit@hotmail.edu",
            BirthDate = new DateTime(2025, 08, 10),
            PhoneNo = "0164-182-401",
            UserId = 20,
            EnrolledYear = 2020
        },
        new StudentVer2 {

            Id = 21,

            StudentCode = "SC00021",

            StudentName = "Charde Velez",

            Email = "ipsum@google.com",

            BirthDate = new DateTime(2025, 04, 12),

            PhoneNo = "0983-687-577",

            UserId = 21,

            EnrolledYear = 2018

        },

        new StudentVer2 {

            Id = 22,

            StudentCode = "SC00022",

            StudentName = "Davis Case",

            Email = "non.hendrerit@protonmail.edu",

            BirthDate = new DateTime(2025, 03, 05),

            PhoneNo = "0169-241-754",

            UserId = 22,

            EnrolledYear = 2017

        },

        new StudentVer2 {

            Id = 23,

            StudentCode = "SC00023",

            StudentName = "Kimberly Gomez",

            Email = "mauris@protonmail.edu",

            BirthDate = new DateTime(2025, 01, 24),

            PhoneNo = "0405-848-957",

            UserId = 23,

            EnrolledYear = 2023

        },

        new StudentVer2 {

            Id = 24,

            StudentCode = "SC00024",

            StudentName = "Amity Pace",

            Email = "lacinia.at@yahoo.com",

            BirthDate = new DateTime(2025, 02, 20),

            PhoneNo = "0268-044-714",

            UserId = 24,

            EnrolledYear = 2023

        },

        new StudentVer2 {

            Id = 25,

            StudentCode = "SC00025",

            StudentName = "Kermit Christian",

            Email = "tincidunt.orci.quis@icloud.ca",

            BirthDate = new DateTime(2024, 06, 02),

            PhoneNo = "0955-427-433",

            UserId = 25,

            EnrolledYear = 2018

        },

        new StudentVer2 {

            Id = 26,

            StudentCode = "SC00026",

            StudentName = "Bradley Vargas",

            Email = "vitae.erat.vivamus@google.edu",

            BirthDate = new DateTime(2025, 05, 11),

            PhoneNo = "0853-424-581",

            UserId = 26,

            EnrolledYear = 2017

        },

        new StudentVer2 {

            Id = 27,

            StudentCode = "SC00027",

            StudentName = "Joshua Christensen",

            Email = "nunc.sed.orci@protonmail.edu",

            BirthDate = new DateTime(2023, 12, 30),

            PhoneNo = "0166-168-314",

            UserId = 27,

            EnrolledYear = 2020

        },

        new StudentVer2 {

            Id = 28,

            StudentCode = "SC00028",

            StudentName = "Lawrence Mcdaniel",

            Email = "fermentum.vel.mauris@aol.net",

            BirthDate = new DateTime(2025, 04, 12),

            PhoneNo = "0171-315-952",

            UserId = 28,

            EnrolledYear = 2021

        },

        new StudentVer2 {

            Id = 29,

            StudentCode = "SC00029",

            StudentName = "Gage Peck",

            Email = "vivamus@google.net",

            BirthDate = new DateTime(2025, 05, 30),

            PhoneNo = "0791-748-743",

            UserId = 29,

            EnrolledYear = 2020

        },

        new StudentVer2 {

            Id = 30,

            StudentCode = "SC00030",

            StudentName = "Nolan Payne",

            Email = "a.feugiat@aol.couk",

            BirthDate = new DateTime(2024, 07, 10),

            PhoneNo = "0360-817-878",

            UserId = 30,

            EnrolledYear = 2022

        },

        new StudentVer2 {

            Id = 31,

            StudentCode = "SC00031",

            StudentName = "Naomi Norris",

            Email = "venenatis@hotmail.ca",

            BirthDate = new DateTime(2025, 07, 05),

            PhoneNo = "0896-822-742",

            UserId = 31,

            EnrolledYear = 2018

        },

        new StudentVer2 {

            Id = 32,

            StudentCode = "SC00032",

            StudentName = "Aurora Kemp",

            Email = "semper.pretium@aol.edu",

            BirthDate = new DateTime(2025, 03, 17),

            PhoneNo = "0841-838-537",

            UserId = 32,

            EnrolledYear = 2018

        },

        new StudentVer2 {

            Id = 33,

            StudentCode = "SC00033",

            StudentName = "Porter Carpenter",

            Email = "tincidunt.donec@outlook.couk",

            BirthDate = new DateTime(2024, 07, 26),

            PhoneNo = "0188-871-228",

            UserId = 33,

            EnrolledYear = 2019

        },

        new StudentVer2 {

            Id = 34,

            StudentCode = "SC00034",

            StudentName = "Alexis Chavez",

            Email = "sed.pede@icloud.ca",

            BirthDate = new DateTime(2024, 11, 03),

            PhoneNo = "0406-821-674",

            UserId = 34,

            EnrolledYear = 2019

        },

        new StudentVer2 {

            Id = 35,

            StudentCode = "SC00035",

            StudentName = "Acton Kinney",

            Email = "aliquet.libero@yahoo.com",

            BirthDate = new DateTime(2023, 11, 08),

            PhoneNo = "0721-753-209",

            UserId = 35,

            EnrolledYear = 2019

        },

        new StudentVer2 {

            Id = 36,

            StudentCode = "SC00036",

            StudentName = "Jillian Bryant",

            Email = "commodo.ipsum@protonmail.net",

            BirthDate = new DateTime(2023, 11, 30),

            PhoneNo = "0170-174-268",

            UserId = 36,

            EnrolledYear = 2023

        },

        new StudentVer2 {

            Id = 37,

            StudentCode = "SC00037",

            StudentName = "Herman Barr",

            Email = "bibendum.fermentum@hotmail.ca",

            BirthDate = new DateTime(2024, 12, 09),

            PhoneNo = "0648-187-769",

            UserId = 37,

            EnrolledYear = 2018

        },

        new StudentVer2 {

            Id = 38,

            StudentCode = "SC00038",

            StudentName = "Keegan Eaton",

            Email = "enim.commodo@icloud.com",

            BirthDate = new DateTime(2024, 04, 07),

            PhoneNo = "0135-144-508",

            UserId = 38,

            EnrolledYear = 2021

        },

        new StudentVer2 {

            Id = 39,

            StudentCode = "SC00039",

            StudentName = "Florence Conrad",

            Email = "pede.nonummy@hotmail.edu",

            BirthDate = new DateTime(2025, 05, 18),

            PhoneNo = "0369-194-553",

            UserId = 39,

            EnrolledYear = 2023

        },

        new StudentVer2 {

            Id = 40,

            StudentCode = "SC00040",

            StudentName = "Aladdin Colon",

            Email = "dolor.quam@yahoo.net",

            BirthDate = new DateTime(2024, 07, 21),

            PhoneNo = "0344-436-336",

            UserId = 40,

            EnrolledYear = 2022

        },

        new StudentVer2 {

            Id = 41,

            StudentCode = "SC00041",

            StudentName = "Oleg Crawford",

            Email = "ut.nec@aol.edu",

            BirthDate = new DateTime(2024, 02, 26),

            PhoneNo = "0166-673-464",

            UserId = 41,

            EnrolledYear = 2024

        },

        new StudentVer2 {

            Id = 42,

            StudentCode = "SC00042",

            StudentName = "Chloe Reed",

            Email = "id@yahoo.org",

            BirthDate = new DateTime(2024, 09, 12),

            PhoneNo = "0350-164-885",

            UserId = 42,

            EnrolledYear = 2022

        },
       new StudentVer2 {
            Id = 43,
            StudentCode = "SC00043",
            StudentName = "David Wheeler",
            Email = "etiam@yahoo.edu",
            BirthDate = new DateTime(2024, 10, 01),
            PhoneNo = "0820-171-591",
            UserId = 43,
            EnrolledYear = 2019
        },
        new StudentVer2 {
            Id = 44,
            StudentCode = "SC00044",
            StudentName = "Illana Alston",
            Email = "in.consequat@icloud.com",
            BirthDate = new DateTime(2025, 01, 05),
            PhoneNo = "0331-260-256",
            UserId = 44,
            EnrolledYear = 2017
        },
        new StudentVer2 {
            Id = 45,
            StudentCode = "SC00045",
            StudentName = "Rana Harper",
            Email = "dui.nec@yahoo.ca",
            BirthDate = new DateTime(2025, 01, 13),
            PhoneNo = "0529-183-781",
            UserId = 45,
            EnrolledYear = 2022
        },
        new StudentVer2 {
            Id = 46,
            StudentCode = "SC00046",
            StudentName = "Patience Moreno",
            Email = "mattis.cras@aol.ca",
            BirthDate = new DateTime(2025, 03, 16),
            PhoneNo = "0360-132-588",
            UserId = 46,
            EnrolledYear = 2023
        },
        new StudentVer2 {
            Id = 47,
            StudentCode = "SC00047",
            StudentName = "Caleb Bradley",
            Email = "pellentesque.massa@yahoo.org",
            BirthDate = new DateTime(2025, 02, 07),
            PhoneNo = "0616-908-681",
            UserId = 47,
            EnrolledYear = 2022
        },
        new StudentVer2 {
            Id = 48,
            StudentCode = "SC00048",
            StudentName = "Zachary Clarke",
            Email = "rutrum.lorem@outlook.org",
            BirthDate = new DateTime(2025, 02, 08),
            PhoneNo = "0115-282-133",
            UserId = 48,
            EnrolledYear = 2019
        },
        new StudentVer2 {
            Id = 49,
            StudentCode = "SC00049",
            StudentName = "Driscoll Castillo",
            Email = "diam.pellentesque@google.edu",
            BirthDate = new DateTime(2024, 04, 25),
            PhoneNo = "0787-157-761",
            UserId = 49,
            EnrolledYear = 2017
        },
        new StudentVer2 {
            Id = 50,
            StudentCode = "SC00050",
            StudentName = "Maya Gay",
            Email = "sem.nulla@protonmail.net",
            BirthDate = new DateTime(2025, 02, 03),
            PhoneNo = "0388-168-111",
            UserId = 50,
            EnrolledYear = 2017
        },

        ];
    }
}
