using System;
using System.Collections.Generic;

namespace 小專後端.Models;

public partial class sqlserver_import_vocabulary_zhheader_utf8bom
{
    public string 分類 { get; set; } = null!;

    public byte 等級 { get; set; }

    public string 單字 { get; set; } = null!;

    public string 詞性 { get; set; } = null!;

    public string 中文意思 { get; set; } = null!;

    public string? 音標 { get; set; }

    public string 克漏字例句 { get; set; } = null!;
}
