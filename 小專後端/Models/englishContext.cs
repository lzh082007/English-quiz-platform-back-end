using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace 小專後端.Models;

public partial class englishContext : DbContext
{
    public englishContext()
    {
    }

    public englishContext(DbContextOptions<englishContext> options)
        : base(options)
    {
    }

    public virtual DbSet<categories> categories { get; set; }

    public virtual DbSet<list_items> list_items { get; set; }

    public virtual DbSet<login_time> login_time { get; set; }

    public virtual DbSet<personal_lists> personal_lists { get; set; }

    public virtual DbSet<problem_list> problem_list { get; set; }

    public virtual DbSet<question_type> question_type { get; set; }

    public virtual DbSet<questions> questions { get; set; }

    public virtual DbSet<quiz_detail_logs> quiz_detail_logs { get; set; }

    public virtual DbSet<quiz_error_option> quiz_error_option { get; set; }

    public virtual DbSet<quiz_records> quiz_records { get; set; }

    public virtual DbSet<sqlserver_import_vocabulary_zhheader_utf8bom> sqlserver_import_vocabulary_zhheader_utf8bom { get; set; }

    public virtual DbSet<users> users { get; set; }

    public virtual DbSet<words> words { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<categories>(entity =>
        {
            entity.HasKey(e => e.cid).HasName("PK__categori__D837D05F9D3E6C6B");

            entity.Property(e => e.categories1)
                .HasMaxLength(20)
                .HasColumnName("categories");
        });

        modelBuilder.Entity<list_items>(entity =>
        {
            entity.HasKey(e => new { e.list_id, e.word_id }).HasName("PK__list_ite__6C6150E1C3EAA6DD");

            entity.Property(e => e.created_at).HasColumnType("datetime");

            entity.HasOne(d => d.list).WithMany(p => p.list_items)
                .HasForeignKey(d => d.list_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_list_items_personal_lists");

            entity.HasOne(d => d.word).WithMany(p => p.list_items)
                .HasForeignKey(d => d.word_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_list_items_words");
        });

        modelBuilder.Entity<login_time>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.login_at).HasColumnType("datetime");

            entity.HasOne(d => d.user).WithMany()
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_login_time_users");
        });

        modelBuilder.Entity<personal_lists>(entity =>
        {
            entity.HasKey(e => e.lid).HasName("PK__personal__DE105D079DE698F9");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.title).HasMaxLength(50);

            entity.HasOne(d => d.user).WithMany(p => p.personal_lists)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_personal_lists_users");
        });

        modelBuilder.Entity<problem_list>(entity =>
        {
            entity.HasKey(e => e.pid).HasName("PK__problem___DD37D91AC81F40CA");

            entity.Property(e => e.created_at)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.question).WithMany(p => p.problem_list)
                .HasForeignKey(d => d.question_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_problem_list_questions");

            entity.HasOne(d => d.reporter).WithMany(p => p.problem_list)
                .HasForeignKey(d => d.reporter_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_problem_list_users");
        });

        modelBuilder.Entity<question_type>(entity =>
        {
            entity.HasKey(e => e.tid).HasName("PK__question__DC105B0FF27D961C");

            entity.Property(e => e.question_type1)
                .HasMaxLength(50)
                .HasColumnName("question_type");
        });

        modelBuilder.Entity<questions>(entity =>
        {
            entity.HasKey(e => e.qid).HasName("PK__question__C277C221E720D568");

            entity.HasOne(d => d.question_type).WithMany(p => p.questions)
                .HasForeignKey(d => d.question_type_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_questions_question_type");

            entity.HasOne(d => d.word).WithMany(p => p.questions)
                .HasForeignKey(d => d.word_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_questions_words");
        });

        modelBuilder.Entity<quiz_detail_logs>(entity =>
        {
            entity.HasKey(e => new { e.quiz_record_id, e.question_id }).HasName("PK__quiz_det__64E3720A8066BD3F");

            entity.Property(e => e.user_answer).HasMaxLength(50);

            entity.HasOne(d => d.question).WithMany(p => p.quiz_detail_logs)
                .HasForeignKey(d => d.question_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_quiz_detail_logs_questions");

            entity.HasOne(d => d.quiz_record).WithMany(p => p.quiz_detail_logs)
                .HasForeignKey(d => d.quiz_record_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_quiz_detail_logs_quiz_records");
        });

        modelBuilder.Entity<quiz_error_option>(entity =>
        {
            entity.HasKey(e => new { e.quiz_record_id, e.question_id, e.option_word_id }).HasName("PK__quiz_err__D17D6CABC35F45B7");

            entity.HasOne(d => d.option_word).WithMany(p => p.quiz_error_option)
                .HasForeignKey(d => d.option_word_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_quiz_error_option_words");

            entity.HasOne(d => d.quiz_detail_logs).WithMany(p => p.quiz_error_optionquiz_detail_logs)
                .HasForeignKey(d => new { d.question_id, d.option_word_id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_quiz_error_option_quiz_detail_logs1");

            entity.HasOne(d => d.quiz_detail_logsNavigation).WithMany(p => p.quiz_error_optionquiz_detail_logsNavigation)
                .HasForeignKey(d => new { d.quiz_record_id, d.question_id })
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_quiz_error_option_quiz_detail_logs");
        });

        modelBuilder.Entity<quiz_records>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__quiz_rec__3213E83F36D5D1B4");

            entity.Property(e => e.quiz_at)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.user).WithMany(p => p.quiz_records)
                .HasForeignKey(d => d.user_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_quiz_records_users");
        });

        modelBuilder.Entity<sqlserver_import_vocabulary_zhheader_utf8bom>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.中文意思).HasMaxLength(100);
            entity.Property(e => e.克漏字例句).HasMaxLength(100);
            entity.Property(e => e.分類).HasMaxLength(50);
            entity.Property(e => e.單字).HasMaxLength(50);
            entity.Property(e => e.詞性).HasMaxLength(50);
            entity.Property(e => e.音標).HasMaxLength(50);
        });

        modelBuilder.Entity<users>(entity =>
        {
            entity.HasKey(e => e.uid).HasName("PK__users__DD701264FB198B0D");

            entity.Property(e => e.auth_code).HasMaxLength(200);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.email).HasMaxLength(100);
            entity.Property(e => e.nickname).HasMaxLength(50);
            entity.Property(e => e.role).HasMaxLength(20);
        });

        modelBuilder.Entity<words>(entity =>
        {
            entity.HasKey(e => e.wid).HasName("PK__words__30F153BBC77FB6E0");

            entity.Property(e => e.KK).HasMaxLength(50);
            entity.Property(e => e.meaning).HasMaxLength(225);
            entity.Property(e => e.parts_of_speech).HasMaxLength(10);
            entity.Property(e => e.spelling).HasMaxLength(50);

            entity.HasOne(d => d.categories).WithMany(p => p.words)
                .HasForeignKey(d => d.categories_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_words_categories");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
