using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObjectHash
{
    class People : IHash
    {
        public long? Id { get; set; }                                   /*unique identifier, avoid it to use in hash calculation*/
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDateTime { get; set; }

        public string HashString()
        {
            var value = new HashHelper().HashString(Name, IsActive, CreatedDateTime);    /*Add more not constant properties as needed*/
            return value;
        }
    }

    [Serializable]
    class PeopleModel
    {
        public long? Id { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDateTime { get; set; }
    }
}
