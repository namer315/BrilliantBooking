using System;
using System.Collections.Generic;
using System.Text;

namespace Data.DTO.Common;

public class EmailDTO
{
    public string To { get; set; } = string.Empty;

    public string Subject { get; set; } = string.Empty;

    public string Body { get; set; } = string.Empty;

    public bool IsHtml { get; set; } = true;
}
