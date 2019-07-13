using System;

namespace ObjectHash
{
    class Program
    {
        static void Main(string[] args)
        {
            /*constant: e6fb7af54c39f39507c28a86ad98a1fd*/
            string name = "Dipon Roy";
            string value = new HashHelper().HashString(name);
            Console.WriteLine(value);


            string hashString;
            /*constant: 47ccecfc14f9ed9eff5de591b8614077*/
            var people = new People();
            hashString = people.HashString();
            /*constant: 3953fbec5b81ccca72c98655c0c4b069*/
            people = new People()
            {
                Id = 1,
                Name = "Dennis Ritchie",
                IsActive = false,
                CreatedDateTime = new DateTime(1941, 9, 9)
            };
            hashString = people.HashString();




            /*--------------------------------- Not good for comparing -----------------------------*/
            var hashHelper = new HashHelper();
            /*throws error as [Serializable] not been used*/
            //var peopleHashString = hashHelper.HashString(people);
            /*constant: 9105d073ad276d742c56a049abd4ddef
             * will change if we change 
             *      1. class name
             *      2. property name
             *      3. property data type
             *      4. add/remove new property
             */
            var peopleModelHashString = hashHelper.HashString(new PeopleModel()
            {
                Id = 1,
                Name = "Anders Hejlsberg",
                IsActive = true,
                CreatedDateTime = new DateTime(1960, 12, 2)
            });

            Console.ReadKey();
        }
    }
}
