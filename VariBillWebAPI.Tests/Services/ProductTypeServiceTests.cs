using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using VariBillWebAPI.Data.Entities;
using VariBillWebAPI.Data.UnitOfWork.Interfaces;
using VariBillWebAPI.Data.Repository.Interfaces;
using VariBillWebAPI.Models.DTO;
using VariBillWebAPI.Services;
using Xunit;

namespace VariBillWebAPI.Tests.Services
{
    public class ProductTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IProductTypeRepository> _productTypeRepoMock;
        private readonly Mock<ILogger<ProductTypeService>> _loggerMock;
        private readonly ProductTypeService _service;

        public ProductTypeServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _productTypeRepoMock = new Mock<IProductTypeRepository>();
            _loggerMock = new Mock<ILogger<ProductTypeService>>();

            _unitOfWorkMock.Setup(u => u.ProductTypes).Returns(_productTypeRepoMock.Object);

            _service = new ProductTypeService(_unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsMappedDtos()
        {
            // Arrange
            var types = new List<(ProductType ProductType, int Count)>
            {
                (new ProductType { Id = Guid.NewGuid(), Name = "A", Description = "desc" }, 2),
                (new ProductType { Id = Guid.NewGuid(), Name = "B", Description = "desc2" }, 0)
            };

            _productTypeRepoMock.Setup(r => r.GetAllWithProductCountsAsync())
                .ReturnsAsync(types);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.Name == "A" && r.ProductCount == 2);
        }

        [Fact]
        public async Task CreateAsync_ValidDto_AddsAndReturnsDto()
        {
            // Arrange
            var dto = new CreateProductTypeDto("NewType", "d");
            ProductType captured = null!;
            _productTypeRepoMock.Setup(r => r.AddAsync(It.IsAny<ProductType>()))
                .ReturnsAsync((ProductType p) =>
                {
                    captured = p;
                    return p;
                });
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(0, result.ProductCount);
            Assert.Equal(captured?.Name, dto.Name.Trim());
        }

        [Fact]
        public async Task UpdateAsync_NonExisting_ReturnsFalse()
        {
            // Arrange
            var id = Guid.NewGuid();
            _productTypeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ProductType?)null);

            // Act
            var result = await _service.UpdateAsync(id, new UpdateProductTypeDto(id, "X", null, true));

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_HasActiveProducts_ReturnsFalseWithMessage()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pt = new ProductType { Id = id, Name = "T" };
            _productTypeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(pt);
            _productTypeRepoMock.Setup(r => r.HasActiveProductsAsync(id)).ReturnsAsync(true);

            // Act
            var (success, message) = await _service.DeleteAsync(id);

            // Assert
            Assert.False(success);
            Assert.False(string.IsNullOrWhiteSpace(message));
        }

        [Fact]
        public async Task DeleteAsync_NoActiveProducts_Succeeds()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pt = new ProductType { Id = id, Name = "T" };
            _productTypeRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(pt);
            _productTypeRepoMock.Setup(r => r.HasActiveProductsAsync(id)).ReturnsAsync(false);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var (success, message) = await _service.DeleteAsync(id);

            // Assert
            Assert.True(success);
            Assert.Null(message);
            Assert.True(pt.IsDeleted);
            Assert.False(pt.IsActive);
        }

        [Fact]
        public async Task CreateAsync_EmptyName_ThrowsValidationException()
        {
            // Arrange
            var dto = new CreateProductTypeDto("   ", null);

            // Act & Assert
            await Assert.ThrowsAsync<System.ComponentModel.DataAnnotations.ValidationException>(() => _service.CreateAsync(dto));
        }
    }
}
