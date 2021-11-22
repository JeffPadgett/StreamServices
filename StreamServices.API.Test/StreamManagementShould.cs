using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Newtonsoft.Json;
using StreamServices.Core;
using StreamServices.Dto;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace StreamServices.API.Test.Unit
{
    public class StreamManagementShould
    {
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;
        private readonly StreamManagement _sut;
        public StreamManagementShould()
        {
            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile())));
            _logger = NullLoggerFactory.Instance.CreateLogger("Test");
            _httpClient = new HttpClient();
        }
        //create http request
        private static Mock<HttpRequest> CreateMockRequest(object body)
        {
            var serializedJson = JsonConvert.SerializeObject(body);
            var memoryStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(serializedJson));
            var mockRequest = new Mock<HttpRequest>();
            mockRequest.Setup(x => x.Body).Returns(memoryStream);
            mockRequest.Setup(x => x.ContentType).Returns("application/json");

            return mockRequest;
        }

        [Fact]
        public async Task StreamManagementShouldSubscribe()
        {
            //Arrange
            var twitchStreamer = new TwitchUser { };

            //Act
            //StreamManagement.Subscribe(CreateMockRequest())
            //Assert
        }

    }
}
