using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using LibraryEngine;

namespace LibraryDataAccess
{
    /// <summary>
    /// The class that initializes the database.
    /// </summary>
    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<LibraryContext>
    {
        /// <summary>
        /// Seeds the database with dummy data.
        /// </summary>
        /// <param name="context">The database's context.</param>
        protected override void Seed(LibraryContext context)
        {
            Guid adminGuid1 = Guid.NewGuid();
            Guid adminGuid2 = Guid.NewGuid();
            Guid adminGuid3 = Guid.NewGuid();

            string adminPassword1 = Security.CreateHashedPassword("carolyn" + adminGuid1.ToString());
            string adminPassword2 = Security.CreateHashedPassword("ahirt" + adminGuid2.ToString());
            string adminPassword3 = Security.CreateHashedPassword("doua" + adminGuid3.ToString());

            var admins = new List<Administrator>
            {
                new Administrator { Username = "carolyntiry", Password = adminPassword1, Guid = adminGuid1 },
                new Administrator { Username = "ahirt", Password = adminPassword2, Guid = adminGuid2 },
                new Administrator { Username = "doualee", Password = adminPassword3, Guid = adminGuid3 }
            };
            context.Administrators.AddRange(admins);
            context.SaveChanges();

            // Create authors
            var authors = new List<Author>
            {
                new Author { FirstName = "Kurt", LastName = "Vonnegut" }, // 1
                new Author { FirstName = "Harper", LastName = "Lee" }, // 2
                new Author { FirstName = "David", LastName = "Shafer" }, // 3
                new Author { FirstName = "Bill", LastName = "Bryson" }, // 4
                new Author { FirstName = "Donna", LastName = "Tartt" }, // 5
                new Author { FirstName = "David", LastName = "Sedaris" }, // 6
                new Author { FirstName = "Ann", LastName = "Patchett" }, // 7
                new Author { FirstName = "Toni", LastName = "Morrison" }, // 8
                new Author { FirstName = "Michael", LastName = "Chabon" }, // 9
                new Author { FirstName = "Thomas", LastName = "Pynchon" } // 10
            };
            context.Authors.AddRange(authors);
            context.SaveChanges();

            // Create genres
            var genres = new List<Genre>
            {
                new Genre { Name = "Humor" }, // 1
                new Genre { Name = "Historical Fiction" }, // 2
                new Genre { Name = "Fantasy" }, // 3
                new Genre { Name = "Science Fiction" }, // 4
                new Genre { Name = "Romance" }, // 5
                new Genre { Name = "Young Adult" }, // 6
                new Genre { Name = "Mystery" }, // 7
                new Genre { Name = "Thriller" }, // 8
                new Genre { Name = "Satire" }, // 9
                new Genre { Name = "Classic" }, // 10
                new Genre { Name = "Travel" }, // 11
                new Genre { Name = "Memoir" }, // 12
                new Genre { Name = "General Fiction" }, // 13
                new Genre { Name = "None" }
            };
            context.Genres.AddRange(genres);
            context.SaveChanges();

            // Create books
            var books = new List<Book>
            {
                new Book { Title = "Slaughterhouse Five", Isbn = "1234567890123", AuthorId = 1, GenreId = 9 }, // 1
                new Book { Title = "Breakfast of Champions", Isbn = "2345678901234", AuthorId = 1, GenreId = 9 }, // 2
                new Book { Title = "To Kill a Mockingbird", Isbn = "3456789012345", AuthorId = 2, GenreId = 13 }, // 3
                new Book { Title = "Go Set a Watchman", Isbn = "4567890123456", AuthorId = 2, GenreId = 13 }, // 4
                new Book { Title = "The Amazing Adventures of Kavalier and Clay", Isbn = "5678901234567", AuthorId = 9, GenreId = 2 }, // 5
                new Book { Title = "The Yiddish Policemen's Union", Isbn = "6789012345678", AuthorId = 9, GenreId = 2 }, // 6
                new Book { Title = "Inherent Vice", Isbn = "7890123456789", AuthorId = 10, GenreId = 7 }, // 7
                new Book { Title = "Whiskey Tango Foxtrot", Isbn = "8901234567890", AuthorId = 3, GenreId = 7 }, // 8
                new Book { Title = "The Secret History", Isbn = "9012345678901", AuthorId = 5, GenreId = 13 }, // 9
                new Book { Title = "Bel Canto", Isbn = "0123456789012", AuthorId = 7, GenreId = 13 }, // 10
                new Book { Title = "A Walk in the Woods", Isbn = "1029384756473", AuthorId = 4, GenreId = 12 } // 11
            };
            context.Books.AddRange(books);
            context.SaveChanges();

            // Create publishers
            var publishers = new List<Publisher>
            {
                new Publisher { Name = "Scholastic", Location = "United States" }, // 1
                new Publisher { Name = "Harper Collins", Location = "United States" }, // 2
                new Publisher { Name = "Simon & Schuster", Location = "United States" }, // 3
                new Publisher { Name = "Pearson", Location = "United Kingdom" }, // 4
                new Publisher { Name = "ThomsonReuters", Location = "Canada" }, // 5
                new Publisher { Name = "Alfred A. Knopf", Location = "United States" }, // 6
                new Publisher { Name = "Penguin Books", Location = "United Kingdom" }, // 7
                new Publisher { Name = "Random House", Location = "United States" } // 8
            };
            context.Publishers.AddRange(publishers);
            context.SaveChanges();

            // Create formats
            var formats = new List<Format>
            {
                new Format { Type = "Paperback" }, // 1
                new Format { Type = "Hardcover" }, // 2
                new Format { Type = "Large Print Paperback" }, // 3
                new Format { Type = "Large Print Hardcover" }, // 4
                new Format { Type = "Audiobook CD" }, // 5
                new Format { Type = "Audiobook MP3" }, // 6
                new Format { Type = "E-book" }, // 7
                new Format { Type = "None" }
            };
            context.Formats.AddRange(formats);
            context.SaveChanges();

            // Create copies
            var copies = new List<BookCopy>
            {
                new BookCopy { BookId = 1, CopyrightYear = 1971, FormatId = 1, NumberOfPages = 268, ShelfNumber = "A12", PublisherId = 7, IsAvailable = true }, // 1
                new BookCopy { BookId = 1, CopyrightYear = 1971, FormatId = 7, NumberOfPages = 299, PublisherId = 7, IsAvailable = true }, // 2
                new BookCopy { BookId = 3, CopyrightYear = 1958, FormatId = 2, NumberOfPages = 251, ShelfNumber = "G9", PublisherId = 2, IsAvailable = true }, // 3
                new BookCopy { BookId = 4, CopyrightYear = 2015, FormatId = 2, NumberOfPages = 281, ShelfNumber = "G9", PublisherId = 2, IsAvailable = true }, // 4
                new BookCopy { BookId = 5, CopyrightYear = 2001, FormatId = 1, NumberOfPages = 413, ShelfNumber = "B1", PublisherId = 3, IsAvailable = true }, // 5
                new BookCopy { BookId = 5, CopyrightYear = 2001, FormatId = 6, PublisherId = 3, IsAvailable = true }, // 6
                new BookCopy { BookId = 5, CopyrightYear = 2001, FormatId = 7, NumberOfPages = 422, PublisherId = 3, IsAvailable = true }, // 7
                new BookCopy { BookId = 11, CopyrightYear = 1996, FormatId = 1, NumberOfPages = 335, ShelfNumber = "D2", PublisherId = 8, IsAvailable = true }, // 8
                new BookCopy { BookId = 11, CopyrightYear = 1996, FormatId = 3, NumberOfPages = 389, ShelfNumber = "F3", PublisherId = 8, IsAvailable = false }, // 9
                new BookCopy { BookId = 8, CopyrightYear = 2014, FormatId = 2, NumberOfPages = 438, ShelfNumber = "G8", PublisherId = 6, IsAvailable = true }, // 10
                new BookCopy { BookId = 8, CopyrightYear = 2014, FormatId = 1, NumberOfPages = 438, ShelfNumber = "A6", PublisherId = 6, IsAvailable = true }, // 11
                new BookCopy { BookId = 7, CopyrightYear = 2009, FormatId = 1, NumberOfPages = 392, ShelfNumber = "B3", PublisherId = 2, IsAvailable = true }, // 12
                new BookCopy { BookId = 7, CopyrightYear = 2011, FormatId = 5, ShelfNumber = "I5", PublisherId = 2, IsAvailable = true } // 13
            };
            context.Copies.AddRange(copies);
            context.SaveChanges();

            Guid guid1 = Guid.NewGuid();
            Guid guid2 = Guid.NewGuid();
            Guid guid3 = Guid.NewGuid();
            Guid guid4 = Guid.NewGuid();
            Guid guid5 = Guid.NewGuid();
            Guid guid6 = Guid.NewGuid();
            Guid guid7 = Guid.NewGuid();
            Guid guid8 = Guid.NewGuid();
            Guid guid9 = Guid.NewGuid();
            Guid guid10 = Guid.NewGuid();
            Guid guid11 = Guid.NewGuid();

            string password1 = Security.CreateHashedPassword("turner" + guid1.ToString());
            string password2 = Security.CreateHashedPassword("bookworm" + guid2.ToString());
            string password3 = Security.CreateHashedPassword("decimal" + guid3.ToString());
            string password4 = Security.CreateHashedPassword("holmes" + guid4.ToString());
            string password5 = Security.CreateHashedPassword("ontheroad" + guid5.ToString());
            string password6 = Security.CreateHashedPassword("slaughterhouse" + guid6.ToString());
            string password7 = Security.CreateHashedPassword("mockingbird" + guid7.ToString());
            string password8 = Security.CreateHashedPassword("clemens" + guid8.ToString());
            string password9 = Security.CreateHashedPassword("detective" + guid9.ToString());
            string password10 = Security.CreateHashedPassword("mrdarcy" + guid10.ToString());
            string password11 = Security.CreateHashedPassword("hogwarts" + guid11.ToString());

            // Create members
            var members = new List<Member>
            {
                new Member { FirstName = "Paige", LastName = "Turner", Username = "paigeturner", Password = password1, Guid = guid1 }, // 1
                new Member { FirstName = "Billy", LastName = "Bookworm", Username = "billybookworm", Password = password2, Guid = guid2 }, // 2
                new Member { FirstName = "Dewey", LastName = "Decimal", Username = "deweydecimal", Password = password3, Guid = guid3 }, // 3
                new Member { FirstName = "Dean", LastName = "Moriarty", Username = "deanmoriarty", Password = password4, Guid = guid4 }, // 4
                new Member { FirstName = "Sal", LastName = "Paradise", Username = "salparadise", Password = password5, Guid = guid5 }, // 5
                new Member { FirstName = "Billy", LastName = "Pilgrim", Username = "billypilgrim", Password = password6, Guid = guid6 }, // 6
                new Member { FirstName = "Atticus", LastName = "Finch", Username = "atticusfinch", Password = password7, Guid = guid7 }, // 7
                new Member { FirstName = "Tom", LastName = "Sawyer", Username = "tomsawyer", Password = password8, Guid = guid8 }, // 8
                new Member { FirstName = "Nancy", LastName = "Drew", Username = "nancydrew", Password = password9, Guid = guid9 }, // 9
                new Member { FirstName = "Elizabeth", LastName = "Bennet", Username = "lizziebennett", Password = password10, Guid = guid10 }, // 10
                new Member { FirstName = "Hermione", LastName = "Granger", Username = "hermionegranger", Password = password11, Guid = guid11 } // 11
            };
            context.Members.AddRange(members);
            context.SaveChanges();

            // Create transactions
            var transactions = new List<Transaction>
            {
                new Transaction { CheckOutDate = DateTime.Parse("9/1/2015"), MemberId = 1 }, // 1
                new Transaction { CheckOutDate = DateTime.Parse("9/1/2015"), MemberId = 2 }, // 2
                new Transaction { CheckOutDate = DateTime.Parse("9/2/2015"), MemberId = 3 }, // 3
                new Transaction { CheckOutDate = DateTime.Parse("9/3/2015"), MemberId = 4 }, // 4
                new Transaction { CheckOutDate = DateTime.Parse("9/4/2015"), MemberId = 5 }, // 5
                new Transaction { CheckOutDate = DateTime.Parse("10/4/2015"), MemberId = 5 } // 6
            };
            context.Transactions.AddRange(transactions);
            context.SaveChanges();

            var details = new List<TransactionDetail>
            {
                new TransactionDetail { BookCopyId = 3, TransactionId = 1, CheckInDate = DateTime.Parse("9/15/2015"), DueDate = DateTime.Parse("9/22/2015") },
                new TransactionDetail { BookCopyId = 4, TransactionId = 1, CheckInDate = DateTime.Parse("9/15/2015"), DueDate = DateTime.Parse("9/22/2015") },
                new TransactionDetail { BookCopyId = 11, TransactionId = 2, CheckInDate = DateTime.Parse("9/19/2015"), DueDate = DateTime.Parse("9/22/2015") },
                new TransactionDetail { BookCopyId = 12, TransactionId = 2, CheckInDate = DateTime.Parse("9/16/2015"), DueDate = DateTime.Parse("9/22/2015") },
                new TransactionDetail { BookCopyId = 6, TransactionId = 3, CheckInDate = DateTime.Parse("9/23/2015"), DueDate = DateTime.Parse("9/23/2015") },
                new TransactionDetail { BookCopyId = 13, TransactionId = 4, CheckInDate = DateTime.Parse("9/18/2015"), DueDate = DateTime.Parse("9/24/2015") },
                new TransactionDetail { BookCopyId = 1, TransactionId = 5, CheckInDate = DateTime.Parse("9/14/2015"), DueDate = DateTime.Parse("9/25/2015") },
                new TransactionDetail { BookCopyId = 8, TransactionId = 5, CheckInDate = DateTime.Parse("9/23/2015"), DueDate = DateTime.Parse("9/25/2015") },
                new TransactionDetail { BookCopyId = 9, TransactionId = 6, CheckInDate = null, DueDate = DateTime.Parse("10/25/2015") }
            };
            context.TransactionDetails.AddRange(details);
            context.SaveChanges();
        }
    }
}