﻿using CQRS.Core.Commands;
using MongoDB.Driver;

namespace Post.Cmd.Api.Commands;

public class EditMessageCommand : BaseCommand
{
    public string Message { get; set; }
}
