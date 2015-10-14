using System.Collections.Generic;
using System.Linq;

namespace Dehydrator
{
    public class MockContainer
    {
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
        [DehydrateReferences]
        public List<MockEntity1> Entities { get; set; }

        public MockContainer()
        {
        }

        public MockContainer(params MockEntity1[] entities)
        {
            Entities = entities.ToList();
        }
    }
}