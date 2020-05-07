using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.API.Models
{
    /*
     * To migrate to DB
     * dotnet tool install EntityFramework (ef)
     * Commnds:
     * dotnet ef migrations add InitialCreate (a name for the migration)
     * then
     * dotnet ef database update
    */
    public class Value
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
