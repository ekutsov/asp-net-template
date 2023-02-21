global using Xunit;
global using FluentAssertions;
global using Moq;
global using System;
global using System.Threading.Tasks;

#if (authorization == JWT)
global using A2SEVEN.Domain.DTO;
global using A2SEVEN.Domain.Interfaces;
global using A2SEVEN.Domain.ViewModels;
global using A2SEVEN.UnitTests.Shared.Constants;
#endif

global using A2SEVEN.Domain.Exceptions;
global using A2SEVEN.Domain.Constants;
