using Bogus;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using PedidosApi.Domain.Entities;
using PedidosApi.Domain.Enums;
using PedidosApi.Domain.Interfaces;
using PedidosApi.Domain.Services;

namespace UnitTests.Services;

 public class PedidoServiceTests
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly IIntegracaoPedidoService _integracaoService;
        private readonly IFeatureFlagService _featureFlagService;
        private readonly PedidoService _pedidoService;
        private readonly Faker _faker;

        public PedidoServiceTests()
        {
            _pedidoRepository = Substitute.For<IPedidoRepository>();
            _integracaoService = Substitute.For<IIntegracaoPedidoService>();
            _featureFlagService = Substitute.For<IFeatureFlagService>();
            _pedidoService = new PedidoService(
                _pedidoRepository,
                _integracaoService,
                _featureFlagService
            );
            _faker = new Faker();
        }

        [Fact]
        public async Task CriarPedido_QuandoPedidoNaoExiste_DeveRetornarPedidoCriado()
        {
            // Arrange
            int pedidoId = _faker.Random.Int(1, 1000);
            int clienteId = _faker.Random.Int(1, 1000);
            
            var itens = new List<Item>
            {
                new Item
                {
                    ProdutoId = _faker.Random.Int(1, 1000),
                    Quantidade = _faker.Random.Int(1, 10),
                    Valor = _faker.Random.Decimal(10, 100)
                }
            };

            _pedidoRepository.ExistePedidoAsync(pedidoId).Returns(false);
            _featureFlagService.IsFeatureEnabledAsync("ReformaTributaria").Returns(false);
            _pedidoRepository.AdicionarAsync(Arg.Any<Pedido>())!.Returns(Task.FromResult(new Pedido(pedidoId, clienteId, itens)));

            // Act
            var resultado = await _pedidoService.CriarPedidoAsync(pedidoId, clienteId, itens);

            // Assert
            resultado.Should().NotBeNull();
            resultado.PedidoId.Should().Be(pedidoId);
            resultado.ClienteId.Should().Be(clienteId);
            resultado.Status.Should().Be(StatusPedido.Criado);
            
            await _pedidoRepository.Received(1).AdicionarAsync(Arg.Any<Pedido>());
            await _pedidoRepository.Received(1).SalvarAsync();
            await _integracaoService.Received(1).EnviarPedidoAsync(Arg.Any<Pedido>());
        }

        [Fact]
        public async Task CriarPedido_QuandoPedidoJaExiste_DeveLancarExcecao()
        {
            // Arrange
            int pedidoId = _faker.Random.Int(1, 1000);
            int clienteId = _faker.Random.Int(1, 1000);
            
            var itens = new List<Item>
            {
                new Item
                {
                    ProdutoId = _faker.Random.Int(1, 1000),
                    Quantidade = _faker.Random.Int(1, 10),
                    Valor = _faker.Random.Decimal(10, 100)
                }
            };

            _pedidoRepository.ExistePedidoAsync(pedidoId).Returns(true);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => 
                _pedidoService.CriarPedidoAsync(pedidoId, clienteId, itens));
            
            await _pedidoRepository.DidNotReceive().AdicionarAsync(Arg.Any<Pedido>());
            await _pedidoRepository.DidNotReceive().SalvarAsync();
            await _integracaoService.DidNotReceive().EnviarPedidoAsync(Arg.Any<Pedido>());
        }

        [Fact]
        public async Task ObterPorId_QuandoPedidoExiste_DeveRetornarPedido()
        {
            // Arrange
            var pedidoId = _faker.Random.Int(1, 1000);
            var clienteId = _faker.Random.Int(1, 1000);
            
            var itens = new List<Item>
            {
                new Item
                {
                    ProdutoId = _faker.Random.Int(1, 1000),
                    Quantidade = _faker.Random.Int(1, 10),
                    Valor = _faker.Random.Decimal(10, 100)
                }
            };

            var pedido = new Pedido(pedidoId, clienteId, itens);
            _pedidoRepository.ObterPorIdAsync(1).Returns(pedido);

            // Act
            var resultado = await _pedidoService.ObterPorIdAsync(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.PedidoId.Should().Be(pedidoId);
            resultado.ClienteId.Should().Be(clienteId);
        }

        [Fact]
        public async Task ObterPorId_QuandoPedidoNaoExiste_DeveRetornarNull()
        {
            // Arrange
            _pedidoRepository.ObterPorIdAsync(1).ReturnsNull();

            // Act
            var resultado = await _pedidoService.ObterPorIdAsync(1);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task ListarPorStatus_DeveRetornarListaDePedidos()
        {
            // Arrange
            var pedidos = new List<Pedido>
            {
                new Pedido(_faker.Random.Int(1, 1000), _faker.Random.Int(1, 1000), new List<Item>
                {
                    new Item
                    {
                        ProdutoId = _faker.Random.Int(1, 1000),
                        Quantidade = _faker.Random.Int(1, 10),
                        Valor = _faker.Random.Decimal(10, 100)
                    }
                }),
                new Pedido(_faker.Random.Int(1, 1000), _faker.Random.Int(1, 1000), new List<Item>
                {
                    new Item
                    {
                        ProdutoId = _faker.Random.Int(1, 1000),
                        Quantidade = _faker.Random.Int(1, 10),
                        Valor = _faker.Random.Decimal(10, 100)
                    }
                })
            };

            _pedidoRepository.ListarPorStatusAsync(StatusPedido.Criado).Returns(pedidos);

            // Act
            var resultado = await _pedidoService.ListarPorStatusAsync(StatusPedido.Criado);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
        }
    }