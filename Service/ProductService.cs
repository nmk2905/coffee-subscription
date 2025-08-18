using Contracts.DTOs;
using Contracts.DTOs.Product;
using Org.BouncyCastle.Ocsp;
using Repo.Models;
using Repositories;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly ProductRepository _productRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly ICloudinaryService _cloudinaryService;
        public ProductService(ProductRepository productRepository, ICloudinaryService cloudinaryService, CategoryRepository categoryRepository)
        {
            _productRepository = productRepository;
            _cloudinaryService = cloudinaryService;
            _categoryRepository = categoryRepository;
        }

        public async Task<ProductResponse> AddProduct(AddProductDTO request)
        {
            if (request.Image == null || request.Image.Length == 0)
            {
                throw new Exception("Image file is required.");
            }

            string fileExtension = Path.GetExtension(request.Image.FileName);
            string newFileName = $"{Guid.NewGuid()}{fileExtension}";

            CloudinaryResponse cloudinaryResult = await _cloudinaryService.UploadImage(newFileName, request.Image);

            if (cloudinaryResult == null)
            {
                throw new Exception("Error uploading image. Please try again.");
            }

            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                CategoryId = request.CategoryId,
                ImageId = cloudinaryResult.PublicImageId,
                ImageUrl = cloudinaryResult.ImageUrl,
            };

            await _productRepository.CreateAsync(product);
            await _productRepository.SaveAsync();

            var productWithCategory = await _productRepository.GetProductById(product.ProductId);

            var productDTO = new ProductDTO
            {
                ProductId = productWithCategory.ProductId,
                Name = productWithCategory.Name,
                Description = productWithCategory.Description,
                Price = productWithCategory.Price,
                Category = productWithCategory.Category?.Name, 
                ImageUrl = productWithCategory.ImageUrl,
            };


            return new ProductResponse
            {
                Message = "Product added successfully",
                Data = productDTO
            };
        }

        //public async Task<List<ProductDTO>> GetAllProductAsync()
        //{
        //    var result = await _productRepository.GetAllProductAsync();

        //    return result?.Select(p => new ProductDTO
        //    {
        //        ProductId = p.ProductId,
        //        Category = p.Category?.Name,
        //        Name = p.Name,
        //        Description = p.Description,
        //        Price = p.Price             
        //    }).ToList() ?? new List<ProductDTO>();
        //}

        public async Task<List<ProductDTO>> GetAllProductCoffeeAsync()
        {
            var result = await _productRepository.GetAllProductCoffeeAsync();

            return result?.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Category = p.Category?.Name,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToList() ?? new List<ProductDTO>();
        }

        public async Task<List<ProductDTO>> GetAllProductFreezeAsync()
        {
            var result = await _productRepository.GetAllProductFreezeAsync();

            return result?.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Category = p.Category?.Name,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToList() ?? new List<ProductDTO>();
        }

        public async Task<List<ProductDTO>> GetAllProductTeaAsync()
        {
            var result = await _productRepository.GetAllProductTeaAsync();

            return result?.Select(p => new ProductDTO
            {
                ProductId = p.ProductId,
                Category = p.Category?.Name,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ImageUrl
            }).ToList() ?? new List<ProductDTO>();
        }

        public async Task<ProductDTO> GetProductById(int id)
        {
            var result = await _productRepository.GetProductById(id);

            return new ProductDTO
            {
                ProductId = result.ProductId,
                Category = result.Category?.Name,
                Name = result.Name,
                Description = result.Description,
                Price = result.Price,
                ImageUrl = result.ImageUrl
            };
        }

        public async Task<ProductResponse> UpdateProduct(UpdateProductDTO request)
        {
            var product = await _productRepository.GetProductById(request.ProductId);

            if (product == null)
            {
                throw new Exception("Product not found.");
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                product.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Description))
                product.Description = request.Description;

            if (request.Price.HasValue && request.Price.Value > 0)
                product.Price = request.Price.Value;

            if (request.CategoryId > 0) 
                product.CategoryId = request.CategoryId;

            if (request.Image != null && request.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(product.ImageId))
                {
                    await _cloudinaryService.DeleteFileAsync(product.ImageId);
                }

                string fileExtension = Path.GetExtension(request.Image.FileName);
                string newFileName = $"{Guid.NewGuid()}{fileExtension}";

                CloudinaryResponse cloudinaryResult = await _cloudinaryService.UploadImage(newFileName, request.Image);

                if (cloudinaryResult == null)
                {
                    throw new Exception("Error uploading new image. Please try again.");
                }

                product.ImageId = cloudinaryResult.PublicImageId;
                product.ImageUrl = cloudinaryResult.ImageUrl;
            }

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveAsync();

            var productDTO = new ProductDTO
            {
                ProductId = product.ProductId,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category?.Name,
                ImageUrl = product.ImageUrl,
            };

            return new ProductResponse
            {
                Message = "Product updated successfully",
                Data = productDTO
            };
        }


    }
}
