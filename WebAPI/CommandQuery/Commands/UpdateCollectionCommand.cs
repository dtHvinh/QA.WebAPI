﻿using WebAPI.CQRS;
using WebAPI.Dto;
using WebAPI.Response;
using WebAPI.Utilities.Result.Base;

namespace WebAPI.CommandQuery.Commands;

public record UpdateCollectionCommand(UpdateCollectionDto Dto) : ICommand<GenericResult<TextResponse>>;
