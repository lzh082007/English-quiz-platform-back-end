using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace 小專後端.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthCodeColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "categories",
                columns: table => new
                {
                    cid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    categories = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__categori__D837D05F9D3E6C6B", x => x.cid);
                });

            migrationBuilder.CreateTable(
                name: "question_type",
                columns: table => new
                {
                    tid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    question_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__question__DC105B0FF27D961C", x => x.tid);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    uid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    nickname = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    anchcode = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__users__DD701264FB198B0D", x => x.uid);
                });

            migrationBuilder.CreateTable(
                name: "words",
                columns: table => new
                {
                    wid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    spelling = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    meaning = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    parts_of_speech = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KK = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    categories_id = table.Column<int>(type: "int", nullable: false),
                    difficulty_level = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__words__30F153BBC77FB6E0", x => x.wid);
                    table.ForeignKey(
                        name: "FK_words_categories",
                        column: x => x.categories_id,
                        principalTable: "categories",
                        principalColumn: "cid");
                });

            migrationBuilder.CreateTable(
                name: "login_time",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    login_at = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.ForeignKey(
                        name: "FK_login_time_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "uid");
                });

            migrationBuilder.CreateTable(
                name: "personal_lists",
                columns: table => new
                {
                    lid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__personal__DE105D079DE698F9", x => x.lid);
                    table.ForeignKey(
                        name: "FK_personal_lists_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "uid");
                });

            migrationBuilder.CreateTable(
                name: "quiz_records",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    source = table.Column<byte>(type: "tinyint", nullable: false),
                    correct_count = table.Column<int>(type: "int", nullable: false),
                    wrong_count = table.Column<int>(type: "int", nullable: false),
                    quiz_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    total_time_spent = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__quiz_rec__3213E83F36D5D1B4", x => x.id);
                    table.ForeignKey(
                        name: "FK_quiz_records_users",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "uid");
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    qid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    word_id = table.Column<int>(type: "int", nullable: false),
                    question_type_id = table.Column<int>(type: "int", nullable: false),
                    question_correct = table.Column<int>(type: "int", nullable: false),
                    question_error = table.Column<int>(type: "int", nullable: false),
                    question_content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__question__C277C221E720D568", x => x.qid);
                    table.ForeignKey(
                        name: "FK_questions_question_type",
                        column: x => x.question_type_id,
                        principalTable: "question_type",
                        principalColumn: "tid");
                    table.ForeignKey(
                        name: "FK_questions_words",
                        column: x => x.word_id,
                        principalTable: "words",
                        principalColumn: "wid");
                });

            migrationBuilder.CreateTable(
                name: "list_items",
                columns: table => new
                {
                    list_id = table.Column<int>(type: "int", nullable: false),
                    word_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__list_ite__6C6150E1C3EAA6DD", x => new { x.list_id, x.word_id });
                    table.ForeignKey(
                        name: "FK_list_items_personal_lists",
                        column: x => x.list_id,
                        principalTable: "personal_lists",
                        principalColumn: "lid");
                    table.ForeignKey(
                        name: "FK_list_items_words",
                        column: x => x.word_id,
                        principalTable: "words",
                        principalColumn: "wid");
                });

            migrationBuilder.CreateTable(
                name: "problem_list",
                columns: table => new
                {
                    pid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    reporter_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    status = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__problem___DD37D91AC81F40CA", x => x.pid);
                    table.ForeignKey(
                        name: "FK_problem_list_questions",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "qid");
                    table.ForeignKey(
                        name: "FK_problem_list_users",
                        column: x => x.reporter_id,
                        principalTable: "users",
                        principalColumn: "uid");
                });

            migrationBuilder.CreateTable(
                name: "quiz_detail_logs",
                columns: table => new
                {
                    quiz_record_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    is_wrong = table.Column<bool>(type: "bit", nullable: false),
                    user_answer = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    time_taken = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__quiz_det__64E3720A8066BD3F", x => new { x.quiz_record_id, x.question_id });
                    table.ForeignKey(
                        name: "FK_quiz_detail_logs_questions",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "qid");
                    table.ForeignKey(
                        name: "FK_quiz_detail_logs_quiz_records",
                        column: x => x.quiz_record_id,
                        principalTable: "quiz_records",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "quiz_error_option",
                columns: table => new
                {
                    quiz_record_id = table.Column<int>(type: "int", nullable: false),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    option_word_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__quiz_err__D17D6CABC35F45B7", x => new { x.quiz_record_id, x.question_id, x.option_word_id });
                    table.ForeignKey(
                        name: "FK_quiz_error_option_quiz_detail_logs",
                        columns: x => new { x.quiz_record_id, x.question_id },
                        principalTable: "quiz_detail_logs",
                        principalColumns: new[] { "quiz_record_id", "question_id" });
                    table.ForeignKey(
                        name: "FK_quiz_error_option_quiz_detail_logs1",
                        columns: x => new { x.question_id, x.option_word_id },
                        principalTable: "quiz_detail_logs",
                        principalColumns: new[] { "quiz_record_id", "question_id" });
                    table.ForeignKey(
                        name: "FK_quiz_error_option_words",
                        column: x => x.option_word_id,
                        principalTable: "words",
                        principalColumn: "wid");
                });

            migrationBuilder.CreateIndex(
                name: "IX_list_items_word_id",
                table: "list_items",
                column: "word_id");

            migrationBuilder.CreateIndex(
                name: "IX_login_time_user_id",
                table: "login_time",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_personal_lists_user_id",
                table: "personal_lists",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_problem_list_question_id",
                table: "problem_list",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_problem_list_reporter_id",
                table: "problem_list",
                column: "reporter_id");

            migrationBuilder.CreateIndex(
                name: "IX_questions_question_type_id",
                table: "questions",
                column: "question_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_questions_word_id",
                table: "questions",
                column: "word_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_detail_logs_question_id",
                table: "quiz_detail_logs",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_error_option_option_word_id",
                table: "quiz_error_option",
                column: "option_word_id");

            migrationBuilder.CreateIndex(
                name: "IX_quiz_error_option_question_id_option_word_id",
                table: "quiz_error_option",
                columns: new[] { "question_id", "option_word_id" });

            migrationBuilder.CreateIndex(
                name: "IX_quiz_records_user_id",
                table: "quiz_records",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_words_categories_id",
                table: "words",
                column: "categories_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "list_items");

            migrationBuilder.DropTable(
                name: "login_time");

            migrationBuilder.DropTable(
                name: "problem_list");

            migrationBuilder.DropTable(
                name: "quiz_error_option");

            migrationBuilder.DropTable(
                name: "personal_lists");

            migrationBuilder.DropTable(
                name: "quiz_detail_logs");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "quiz_records");

            migrationBuilder.DropTable(
                name: "question_type");

            migrationBuilder.DropTable(
                name: "words");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "categories");
        }
    }
}
