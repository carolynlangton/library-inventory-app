using System;
using System.Collections.Generic;
using LibraryEngine;

namespace LibraryDataAccess
{
    /// <summary>
    /// The class that represents a connection to a database.
    /// </summary>
    public class Repository
    {
        /// <summary>
        /// The repository's database context.
        /// </summary>
        private LibraryContext db = new LibraryContext();

        /// <summary>
        /// Handles the event of an author being added.
        /// </summary>
        public event EventHandler<AuthorEventArgs> AuthorAdded;

        /// <summary>
        /// Handles the event of an author being removed.
        /// </summary>
        public event EventHandler<AuthorEventArgs> AuthorArchived;

        /// <summary>
        /// Handles the event of a book being added.
        /// </summary>
        public event EventHandler<BookEventArgs> BookAdded;

        /// <summary>
        /// Handles the event of a book being removed.
        /// </summary>
        public event EventHandler<BookEventArgs> BookArchived;

        /// <summary>
        /// Handles the event of a copy being added.
        /// </summary>
        public event EventHandler<BookCopyEventArgs> CopyAdded;

        /// <summary>
        /// Handles the event of a copy being removed.
        /// </summary>
        public event EventHandler<BookCopyEventArgs> CopyArchived;

        /// <summary>
        /// Handles the event of a book format being added.
        /// </summary>
        public event EventHandler<FormatEventArgs> FormatAdded;

        /// <summary>
        /// Handles the event of a book format being removed.
        /// </summary>
        public event EventHandler<FormatEventArgs> FormatArchived;

        /// <summary>
        /// Handles the event of a genre being added.
        /// </summary>
        public event EventHandler<GenreEventArgs> GenreAdded;

        /// <summary>
        /// Handles the event of a genre being removed.
        /// </summary>
        public event EventHandler<GenreEventArgs> GenreArchived;

        /// <summary>
        /// Handles the event of a publisher being added.
        /// </summary>
        public event EventHandler<PublisherEventArgs> PublisherAdded;

        /// <summary>
        /// Handles the event of a book being removed.
        /// </summary>
        public event EventHandler<PublisherEventArgs> PublisherArchived;

        /// <summary>
        /// Handles the event of a member being added.
        /// </summary>
        public event EventHandler<MemberEventArgs> MemberAdded;

        /// <summary>
        /// Handles the event of a member being removed.
        /// </summary>
        public event EventHandler<MemberEventArgs> MemberRemoved;

        /// <summary>
        /// Handles the event of a book being removed.
        /// </summary>
        public event EventHandler<MemberEventArgs> MemberArchived;

        /// <summary>
        /// Handles the event of a transaction being added.
        /// </summary>
        public event EventHandler<TransactionEventArgs> TransactionAdded;

        /// <summary>
        /// Raises an event when a transaction detail is added.
        /// </summary>
        public event EventHandler<TransactionDetailEventArgs> TransactionDetailAdded;

        /// <summary>
        /// Adds an author.
        /// </summary>
        /// <param name="author">The author to add.</param>
        public void AddAuthor(Author author)
        {
            // Reference DbSet instead of list
            // Prevents the same object from being added repeatedly
            if (!this.ContainsAuthor(author))
            {
                this.db.Authors.Add(author);

                if (this.AuthorAdded != null)
                {
                    this.AuthorAdded(this, new AuthorEventArgs(author));
                }
            }
        }

        /// <summary>
        /// Adds a book.
        /// </summary>
        /// <param name="book">The book to add.</param>
        public void AddBook(Book book)
        {
            // Prevents the same object from being added repeatedly
            if (!this.ContainsBook(book))
            {
                this.db.Books.Add(book);

                if (this.BookAdded != null)
                {
                    this.BookAdded(this, new BookEventArgs(book));
                }
            }
        }

        /// <summary>
        /// Adds a member.
        /// </summary>
        /// <param name="member">The member to add.</param>
        public void AddMember(Member member)
        {
            // Prevents the same object from being added repeatedly
            if (!this.ContainsMember(member))
            {
                // Hash and salt the password before storing it
                member.Password = Security.CreateHashedPassword(member.Password + member.Guid);
                this.db.Members.Add(member);

                if (this.MemberAdded != null)
                {
                    this.MemberAdded(this, new MemberEventArgs(member));
                }
            }
        }

        /// <summary>
        /// Adds a publisher.
        /// </summary>
        /// <param name="publisher">The publisher to add.</param>
        public void AddPublisher(Publisher publisher)
        {
            // Prevents the same object from being added repeatedly
            if (!this.ContainsPublisher(publisher))
            {
                this.db.Publishers.Add(publisher);

                if (this.PublisherAdded != null)
                {
                    this.PublisherAdded(this, new PublisherEventArgs(publisher));
                }
            }
        }

        /// <summary>
        /// Adds a copy of a book.
        /// </summary>
        /// <param name="copy">The copy to add.</param>
        public void AddCopy(BookCopy copy)
        {
            if (!this.ContainsCopy(copy))
            {
                this.db.Copies.Add(copy);

                if (this.CopyAdded != null)
                {
                    this.CopyAdded(this, new BookCopyEventArgs(copy));
                }
            }
        }

        /// <summary>
        /// Adds a book format.
        /// </summary>
        /// <param name="format">The type of format to add.</param>
        public void AddFormat(Format format)
        {
            // Reference DbSet instead of list
            // Prevents the same object from being added repeatedly.
            if (!this.ContainsFormat(format))
            {
                this.db.Formats.Add(format);

                if (this.FormatAdded != null)
                {
                    this.FormatAdded(this, new FormatEventArgs(format));
                }
            }
        }

        /// <summary>
        /// Adds a genre of a book.
        /// </summary>
        /// <param name="genre">The genre to add.</param>
        public void AddGenre(Genre genre)
        {
            // Reference DbSet instead of list
            if (!this.ContainsGenre(genre))
            {
                this.db.Genres.Add(genre);

                if (this.GenreAdded != null)
                {
                    this.GenreAdded(this, new GenreEventArgs(genre));
                }
            }
        }

        /// <summary>
        /// Adds a transaction.
        /// </summary>
        /// <param name="t">The transaction to add.</param>
        public void AddTransaction(Transaction t)
        {
            if (!this.ContainsTransaction(t))
            {
                this.db.Transactions.Add(t);

                if (this.TransactionAdded != null)
                {
                    this.TransactionAdded(this, new TransactionEventArgs(t));
                }
            }
        }

        /// <summary>
        /// Adds a transaction detail.
        /// </summary>
        /// <param name="td">The transaction detail to add.</param>
        public void AddTransactionDetail(TransactionDetail td)
        {
            if (!this.ContainsTransactionDetail(td))
            {
                this.db.TransactionDetails.Add(td);

                if (this.TransactionDetailAdded != null)
                {
                    this.TransactionDetailAdded(this, new TransactionDetailEventArgs(td));
                }
            }
        }

        /// <summary>
        /// Checks to see if the repository contains the specified author.
        /// </summary>
        /// <param name="author">The author to check.</param>
        /// <returns>A value indicating whether or not the author is in the repository.</returns>
        public bool ContainsAuthor(Author author)
        {
            // Reference DbSet instead of list
            return this.GetAuthor(author.AuthorId) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified book.
        /// </summary>
        /// <param name="book">The book to check.</param>
        /// <returns>A value indicating whether or not the book is in the repository.</returns>
        public bool ContainsBook(Book book)
        {
            return this.GetBook(book.Id) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified publisher.
        /// </summary>
        /// <param name="publisher">The publisher to check.</param>
        /// <returns>A value indicating whether or not the publisher is in the repository.</returns>
        public bool ContainsPublisher(Publisher publisher)
        {
            return this.GetPublisher(publisher.Id) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified member.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>A value indicating whether or not the member is in the repository.</returns>
        public bool ContainsMember(Member member)
        {
            return this.GetMember(member.Id) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified copy.
        /// </summary>
        /// <param name="copy">The copy to check.</param>
        /// <returns>A value indicating whether or not the copy is in the repository.</returns>
        public bool ContainsCopy(BookCopy copy)
        {
            return this.GetCopy(copy.Id) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified format.
        /// </summary>
        /// <param name="format">The format to check.</param>
        /// <returns>A value indicating whether or not the format is in the repository.</returns>
        public bool ContainsFormat(Format format)
        {
            // Reference DbSet instead of list
            return this.GetFormat(format.FormatId) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified genre.
        /// </summary>
        /// <param name="genre">The genre to check.</param>
        /// <returns>A value indicating whether or not the copy is in the repository.</returns>
        public bool ContainsGenre(Genre genre)
        {
            // Reference DbSet instead of list
            return this.GetGenre(genre.GenreId) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified transaction.
        /// </summary>
        /// <param name="t">The transaction to check.</param>
        /// <returns>A value indicating whether or not the transaction is in the repository.</returns>
        public bool ContainsTransaction(Transaction t)
        {
            return this.GetTransaction(t.Id) != null;
        }

        /// <summary>
        /// Checks to see if the repository contains the specified transaction detail.
        /// </summary>
        /// <param name="td">The transaction detail to check.</param>
        /// <returns>A value indicating whether or not the transaction detail is in the repository.</returns>
        public bool ContainsTransactionDetail(TransactionDetail td)
        {
            return this.GetTransactionDetail(td.Id) != null;
        }

        /// <summary>
        /// Gets the author with the specified last name.
        /// </summary>
        /// <param name="id">The last name of the author to find.</param>
        /// <returns>The found author.</returns>
        public Author GetAuthor(int id)
        {
            return this.db.Authors.Find(id);
        }

        /// <summary>
        /// Gets the book with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the book to find.</param>
        /// <returns>The found book.</returns>
        public Book GetBook(int id)
        {
            return this.db.Books.Find(id);
        }

        /// <summary>
        /// Gets the member with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the member to find.</param>
        /// <returns>The found member.</returns>
        public Member GetMember(int id)
        {
            return this.db.Members.Find(id);
        }

        /// <summary>
        /// Gets the publisher with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the publisher to find.</param>
        /// <returns>The found publisher.</returns>
        public Publisher GetPublisher(int id)
        {
            return this.db.Publishers.Find(id);
        }

        /// <summary>
        /// Gets the copy with the specified ID.
        /// </summary>
        /// <param name="id">The id of the copy to find.</param>
        /// <returns>The found copy.</returns>
        public BookCopy GetCopy(int id)
        {
            return this.db.Copies.Find(id);
        }

        /// <summary>
        /// Gets the format of books with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the format to find.</param>
        /// <returns>The format type found.</returns>
        public Format GetFormat(int id)
        {
            return this.db.Formats.Find(id);
        }

        /// <summary>
        /// Gets the genre of books with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the genre to find.</param>
        /// <returns>The specified genre found.</returns>
        public Genre GetGenre(int id)
        {
            return this.db.Genres.Find(id);
        }

        /// <summary>
        /// Gets the transaction with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the transaction to find.</param>
        /// <returns>The found transaction.</returns>
        public Transaction GetTransaction(int id)
        {
            return this.db.Transactions.Find(id);
        }

        /// <summary>
        /// Gets the transaction detail with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the transaction detail to find.</param>
        /// <returns>The found transaction detail.</returns>
        public TransactionDetail GetTransactionDetail(int id)
        {
            return this.db.TransactionDetails.Find(id);
        }

        /// <summary>
        /// Gets a list of all administrators in the repository.
        /// </summary>
        /// <returns>The list of administrators.</returns>
        public List<Administrator> GetAdministrators()
        {
            return new List<Administrator>(this.db.Administrators);
        }

        /// <summary>
        /// Gets a list of all authors in the repository.
        /// </summary>
        /// <returns>The list of authors.</returns>
        public List<Author> GetAuthors()
        {
            return new List<Author>(this.db.Authors);
        }

        /// <summary>
        /// Gets a list of all books in the repository.
        /// </summary>
        /// <returns>The list of books.</returns>
        public List<Book> GetBooks()
        {
            return new List<Book>(this.db.Books);
        }

        /// <summary>
        /// Gets a copy list of all publishers in the database.
        /// </summary>
        /// <returns>The list of publishers.</returns>
        public List<Publisher> GetPublishers()
        {
            return new List<Publisher>(this.db.Publishers);
        }

        /// <summary>
        /// Gets a copy list of all members in the database.
        /// </summary>
        /// <returns>The list of members.</returns>
        public List<Member> GetMembers()
        {
            return new List<Member>(this.db.Members);
        }

        /// <summary>
        /// Gets a list of all copies in the database.
        /// </summary>
        /// <returns>The list of copies.</returns>
        public List<BookCopy> GetCopies()
        {
            return new List<BookCopy>(this.db.Copies);
        }

        /// <summary>
        /// Gets a list of all format types in the database.
        /// </summary>
        /// <returns>The list of format types.</returns>
        public List<Format> GetFormats()
        {
            return new List<Format>(this.db.Formats);
        }

        /// <summary>
        /// Gets a list of all genres in the database.
        /// </summary>
        /// <returns>The list of genres.</returns>
        public List<Genre> GetGenres()
        {
            return new List<Genre>(this.db.Genres);
        }

        /// <summary>
        /// Gets a list of all transactions in the database.
        /// </summary>
        /// <returns>The list of transactions.</returns>
        public List<Transaction> GetTransactions()
        {
            return new List<Transaction>(this.db.Transactions);
        }

        /// <summary>
        /// Gets a list of all transaction details in the database.
        /// </summary>
        /// <returns>The list of transaction details.</returns>
        public List<TransactionDetail> GetTransactionDetails()
        {
            return new List<TransactionDetail>(this.db.TransactionDetails);
        }

        /// <summary>
        /// Removes an author.
        /// </summary>
        /// <param name="author">The author to remove.</param>
        public void ArchiveAuthor(Author author)
        {
            if (this.AuthorArchived != null)
            {
                this.AuthorArchived(this, new AuthorEventArgs(author));
            }

            author.IsArchived = true;
        }

        /// <summary>
        /// Removes a book.
        /// </summary>
        /// <param name="book">The book to remove.</param>
        public void ArchiveBook(Book book)
        {
            if (this.BookArchived != null)
            {
                this.BookArchived(this, new BookEventArgs(book));
            }

            book.IsArchived = true;
        }

        /// <summary>
        /// Removes a book copy.
        /// </summary>
        /// <param name="copy">The copy to remove.</param>
        public void ArchiveCopy(BookCopy copy)
        {
            if (this.CopyArchived != null)
            {
                this.CopyArchived(this, new BookCopyEventArgs(copy));
            }

            copy.IsArchived = true;
        }

        /// <summary>
        /// Removes a format.
        /// </summary>
        /// <param name="format">The format to remove.</param>
        public void ArchiveFormat(Format format)
        {
            if (this.FormatArchived != null)
            {
                this.FormatArchived(this, new FormatEventArgs(format));
            }

            format.IsArchived = true;
        }

        /// <summary>
        /// Removes a genre.
        /// </summary>
        /// <param name="genre">The genre to remove.</param>
        public void ArchiveGenre(Genre genre)
        {
            if (this.GenreArchived != null)
            {
                this.GenreArchived(this, new GenreEventArgs(genre));
            }

            genre.IsArchived = true;
        }

        /// <summary>
        /// Removes a member.
        /// </summary>
        /// <param name="member">The member to remove.</param>
        public void RemoveMember(Member member)
        {
            if (this.MemberRemoved != null)
            {
                this.MemberRemoved(this, new MemberEventArgs(member));
            }

            this.db.Members.Remove(member);
        }

        /// <summary>
        /// Removes a member.
        /// </summary>
        /// <param name="member">The member to remove.</param>
        public void ArchiveMember(Member member)
        {
            if (this.MemberArchived != null)
            {
                this.MemberArchived(this, new MemberEventArgs(member));
            }

            member.IsArchived = true;
        }

        /// <summary>
        /// Removes a publisher.
        /// </summary>
        /// <param name="publisher">The publisher to remove.</param>
        public void ArchivePublisher(Publisher publisher)
        {
            if (this.PublisherArchived != null)
            {
                this.PublisherArchived(this, new PublisherEventArgs(publisher));
            }

            // Doesn't delete data from the database.  Only sets is archived flag to true.
            publisher.IsArchived = true;
        }

        /// <summary>
        /// Saves the changes made and adds to library context.
        /// </summary>
        public void SaveToDatabase()
        {
            this.db.SaveChanges();
        }
    }
}