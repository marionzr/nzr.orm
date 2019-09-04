using Nzr.Orm.Core.Sql;
using Nzr.Orm.Core.Tests.Models;
using Orm.Core.Tests.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Xunit;

namespace Nzr.Orm.Core.Tests
{
    public class DaoTest
    {
        private readonly string connectionStrings;

        public DaoTest()
        {
            string projectDirectory = Path.GetDirectoryName(GetType().Assembly.Location);
            string dbFile = projectDirectory + @"\Assets\Database\Nzr.Orm.Core.mdf";
            connectionStrings = @"Data Source=(localdb)\MSSQLLocalDB;Integrated Security=True; AttachDbFilename=" + dbFile;

            InitDataBase();
        }

        private void InitDataBase()
        {
            using (SqlConnection connection = new SqlConnection(connectionStrings))
            {
                connection.Open();

                using (SqlCommand deleteProductCommand = new SqlCommand("DELETE FROM [core].product WHERE 1=1", connection))
                {
                    deleteProductCommand.ExecuteNonQuery();
                }

                using (SqlCommand deleteCatergoryCommand = new SqlCommand("DELETE FROM [core].category WHERE 1=1", connection))
                {
                    deleteCatergoryCommand.ExecuteNonQuery();
                }

                using (SqlCommand deleteMessCommand = new SqlCommand("DELETE FROM a_mess_table WHERE 1=1", connection))
                {
                    deleteMessCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        #region Insert test

        [Fact]
        public void Insert_EntityWithIdentityDisabled_ShouldInsertAndReturn1AsAffectedRow()
        {
            // Arrange

            Category category = new Category()
            {
                Id = 1,
                Description = "Sports",
                CreatedAt = DateTime.Now
            };

            int expected = 1;
            int result = 0;

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Insert(category);
            }

            // Assert

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Insert_EntityWithIdentityDisabledSameId_ShouldFail()
        {
            // Arrange

            Category category1 = new Category()
            {
                Id = int.MaxValue,
                Description = "Sports",
                CreatedAt = DateTime.Now
            };

            Category category2 = new Category()
            {
                Id = int.MaxValue,
                Description = "Software",
                CreatedAt = DateTime.Now
            };

            // Act

            SqlException exception = Assert.Throws<SqlException>(() =>
            {
                using (Dao dao = new Dao(connectionStrings, "core"))
                {
                    dao.Insert(category1);
                    dao.Insert(category2);
                }
            });

            // Assert

            Assert.Contains("Violation of PRIMARY KEY constraint", exception.Message);
        }

        [Fact]
        public void Insert_EntityWithIdentityEnabled_ShouldInsertAndReturnTheIdentity()
        {
            // Arrange

            Product product1 = new Product()
            {
                Description = "Soccer ball",
                Price = 1.99
            };

            Product product2 = new Product()
            {
                Description = "Soccer nets",
                Price = 2.99
            };

            int result1 = 0;
            int result2 = 0;

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result1 = dao.Insert(product1);
                result2 = dao.Insert(product2);
            }

            // Assert

            Assert.True(result2 > result1);
            Assert.Equal(result1, product1.Id);
            Assert.Equal(result2, product2.Id);
        }

        [Fact]
        public void Insert_EntityWithMessyTableDefinition_ShouldInsertUsingOrmNames()
        {
            // Arrange

            Mess mess = new Mess()
            {
                Description = "A messy has to be fixed.",
                Detail = "Some columns doesn't follow a naming convention",
                CreatedAt = DateTime.Now
            };

            int expected = 0;

            // Act

            using (Dao dao = new Dao(connectionStrings: connectionStrings))
            {
                expected = dao.Insert(mess);
            }

            // Assert

            Assert.Equal(expected, mess.Id);
        }

        #endregion

        #region Select test
        [Fact]
        public void Select_EntityWithValidIds_ShouldReturnEntity()
        {
            // Arrange

            int id = new Random(DateTime.Now.Millisecond).Next(1, int.MaxValue);

            Category category = new Category()
            {
                Id = id,
                Description = "Sports",
                CreatedAt = DateTime.Now
            };


            int result = 0;

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Insert(category);
            }

            Assert.True(result > 0);

            Category existingCategory = null;

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                existingCategory = dao.Select<Category>(id);
            }

            // Assert

            Assert.NotNull(existingCategory);
            Assert.Equal(category.Id, existingCategory.Id);
            Assert.Equal(category.Description, existingCategory.Description);
            Assert.Equal(category.CreatedAt.ToString("dd/M/yy HH:mm:ss"), existingCategory.CreatedAt.ToString("dd/M/yy HH:mm:ss"));
        }

        [Fact]
        public void Select_EntityWitInvalidIds_ShouldReturnNull()
        {
            // Arrange

            Category existingCategory = null;

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                existingCategory = dao.Select<Category>(0);
            }

            // Assert

            Assert.Null(existingCategory);
        }

        [Fact]
        public void Select_EntityWitCustomWhere_ShouldReturnEntities()
        {
            // Arrange

            Product product1 = new Product()
            {
                Description = "Soccer ball",
                Price = 9.99,
                Characteristics = "<characteristic><brand>NZR</brand></characteristic>"
            };

            Product product2 = new Product()
            {
                Description = "Soccer nets",
                Price = 9.99
            };

            Product product3 = new Product()
            {
                Description = "Soccer shoes",
                Price = 1.99
            };

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                dao.Insert(product1);
                dao.Insert(product2);
                dao.Insert(product3);
            }

            IList<Product> productsWithPriceGreaterThan2;
            int expectedProductsWithPriceGreaterThan2 = 2;

            IList<Product> productsWithCharacteristics;
            int expectedproductsWithCharacteristics = 1;
            string characteristicBranc = "NZR";

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                productsWithPriceGreaterThan2 = dao.Select<Product>(new Where { { "Price", Where.GE, 9.99 } });
                productsWithCharacteristics = dao.Select<Product>(new Where { { "Characteristics", Where.IS_NOT, null } });
            }

            // Assert

            Assert.Equal(expectedProductsWithPriceGreaterThan2, productsWithPriceGreaterThan2.Count);
            Assert.Equal(expectedproductsWithCharacteristics, productsWithCharacteristics.Count);
            Assert.Equal(characteristicBranc, productsWithCharacteristics[0].Characteristics.characteristic.brand.ToString());
        }

        #endregion

        #region Update

        [Fact]
        public void Update_Entity_ShouldChangeAttributes()
        {
            // Arrange

            Product product1 = new Product()
            {
                Description = "Soccer glove",
                Price = 1.99
            };

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                dao.Insert(product1);
            }

            int result;

            // Act

            product1.Price = 0.98;

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Update(product1);
            }

            // Assert

            Assert.Equal(1, result);

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                Product product = dao.Select<Product>(product1.Id);
                Assert.Equal(product1.Price, product.Price);
            }
        }

        [Fact]
        public void Update_Fields_ShouldChangeAttributes()
        {
            // Arrange

            Product product1 = new Product()
            {
                Description = "Soccer whistle",
                Price = 0.99
            };

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                dao.Insert(product1);
            }

            int result;

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Update<Product>(new Set { { "Characteristics", "<characteristic><color>black</color></characteristic>" } }, new Where { { "Description", Where.EQ, "Soccer whistle" } });
            }

            // Assert

            Assert.Equal(1, result);
            string expectedCharacteristicColor = "black";
            Product product;

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                product = dao.Select<Product>(product1.Id);
            }

            Assert.Equal(expectedCharacteristicColor, product.Characteristics.characteristic.color.ToString());
        }

        #endregion

        #region Delete

        [Fact]
        public void Delete_Entity_ShouldDeleteIt()
        {
            // Arrange

            Product product1 = new Product()
            {
                Description = "Soccer glove",
                Price = 1.99
            };

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                dao.Insert(product1);
            }

            int result;

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Delete(product1);
            }

            // Assert

            Assert.Equal(1, result);

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                Product product = dao.Select<Product>(product1.Id);
                Assert.Null(product);
            }
        }

        [Fact]
        public void Delete_WithCustomFields_ShouldDelete()
        {
            // Arrange

            Product product1 = new Product()
            {
                Description = "Soccer net",
                Price = 0.99
            };

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                dao.Insert(product1);
            }

            int result;

            // Act

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Delete<Product>(new Where { { "Description", Where.EQ, "Soccer net" } });
            }

            // Assert

            Assert.Equal(1, result);

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                Product product = dao.Select<Product>(product1.Id);
                Assert.Null(product);
            }
        }

        #endregion

        #region Aggregate
        [Fact]
        public void Aggregate_WithSumOfPrices_ShouldReturnTotalOfProductPrices()
        {
            // Arrange

            Product product1 = new Product()
            {
                Description = "Soccer ball",
                Price = 1.99
            };

            Product product2 = new Product()
            {
                Description = "Soccer nets",
                Price = 2.99
            };


            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                dao.Insert(product1);
                dao.Insert(product2);
            }


            double expected = product1.Price + product2.Price;

            // Act
            double result;

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Aggregate<Product, double>(new Aggregate(Aggregate.SUM, "Price"));
            }

            // Assert

            Assert.Equal(expected, result);
        }

        [Fact]
        public void Aggregate_CountCategories_ShouldReturn0()
        {
            // Arrange

            int expected = 0;

            // Act
            int result;

            using (Dao dao = new Dao(connectionStrings, "core"))
            {
                result = dao.Aggregate<Category, int>(new Aggregate(Aggregate.COUNT, "Id"));
            }

            // Assert

            Assert.Equal(expected, result);
        }
        #endregion
    }
}
