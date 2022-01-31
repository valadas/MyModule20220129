using Eraware.Modules.MyModule;
using Eraware.Modules.MyModule.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests
{
    public class EntensionMethodTests : FakeDataContext
    {
        [Fact]
        public void IQueryable_IsOrdered_ThrowsWithNull()
        {
            IQueryable<Item> items = null;

            Action sut = () =>
            {
                var isOrdered = items.IsOrdered();
            };

            Assert.Throws<ArgumentNullException>(sut);
        }

        [Theory]
        [InlineData(false, false, false)]
        [InlineData(false, false, true)]
        [InlineData(false, true, false)]
        [InlineData(false, true, true)]
        [InlineData(true, false, false)]
        [InlineData(true, false, true)]
        [InlineData(true, true, false)]
        [InlineData(true, true, true)]
        public void IQueryable_IsOrdered_Works(bool ordered, bool descending, bool lastOrder)
        {
            GenerateItems(10);
            var items = this.dataContext.Items.AsQueryable();
            if (ordered)
            {
                if (descending)
                {
                    items = items.OrderByDescending(i => i.Name);
                }
                else
                {
                    items = items.OrderBy(i => i.Name);
                }
            }

            if (!lastOrder)
            {
                items = items.Where(i => i.Id > 4);
            }

            Assert.Equal(ordered, items.IsOrdered());
        }

        private void GenerateItems(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                this.dataContext.Items.Add(new Item()
                {
                    CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                    CreatedByUserId = 123,
                    Description = $"Description {1}",
                    Id = i,
                    Name = $"Name {i}",
                    UpdatedAt = DateTime.UtcNow,
                    UpdatedByUserId = 234,
                });
            }
            this.dataContext.SaveChanges();
        }
    }
}
