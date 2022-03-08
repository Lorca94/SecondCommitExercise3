using System.Security.Cryptography;

// Se crea un List para guardar usuarios
List<Users> userRegistered = new List<Users>();
// Se crea el usuario a registrar
Users newUser = new Users();
// Se dan valores al user
newUser.email = "email@email.com";
newUser.password = "1234";
// Se intenta registrar el usuario
userRegistered = RegisterUsers(newUser, userRegistered);
Users newUser2 = new Users();
newUser2.email = "email@email.com";
newUser2.password = "1234";
Console.WriteLine(Login(newUser2, userRegistered));


/*
 * RegisterUser registrará los usuarios en un list y devolverá la lista modificada en caso de poder incluir el user
 */ 
static List<Users> RegisterUsers(Users user, List<Users> userRegistered)
{
    // Condición: Si hay algún usuario registrado
    if (userRegistered.Count != 0)
    {
        // Se itera sobre userRegistered para conmprobar emails
        for(int i = 0; i < userRegistered.Count; i++)
        {
            if (userRegistered[i].email == user.email)
            {
                // Se muestra una razón por la que no se ha insertado en el array y devuelve el array sin modificar
                Console.WriteLine("Email ya registrado anteriormente.");
                return userRegistered;
            }
        }
    }
    //================================ HASH ==================================== --> https://stackoverflow.com/questions/4181198/how-to-hash-a-password

    // Creación de variable para el hash
    byte[] salt;
    // Se llama a NRGCrypto
    new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
    // Tipo de codificación y obtiene el valore del hash
    var pbkdf2 = new Rfc2898DeriveBytes(user.password, salt, 100000);
    byte[] hash = pbkdf2.GetBytes(20);
    // Se combina hash con salt
    byte[] hashBytes = new byte[36];
    Array.Copy(salt, 0, hashBytes, 0, 16);
    Array.Copy(hash, 0, hashBytes, 16, 20);
    // Se guarda la pw hasheada en una variable
    string savedPasswordHash = Convert.ToBase64String(hashBytes);
    // Se modifica la password por la hasheada
    user.password = savedPasswordHash;
    // Se añade al list
    userRegistered.Add(user);
    Console.WriteLine("Usuario registrado con éxito");
    return userRegistered;
}

/*
 * Se comprobará si la contraseña introducida por el usuario es correcta o no
*/ 
static bool Login(Users user, List<Users> userRegistered)
{
    bool log = false;
    foreach(Users DBUser in userRegistered)
    {
        // Se obtiene el password guardado
        string savePasswordHash = DBUser.password;

        // Extracción de bytes
        byte[] hashBytes = Convert.FromBase64String(savePasswordHash);
        // Obtenemos el salt
        byte[] salt = new byte[16];
        Array.Copy(hashBytes, 0, salt, 0, 16);
        //Se calcula el hash de la contraseña que ingresó el usuario
        var pbkdf2 = new Rfc2898DeriveBytes(user.password, salt, 100000);
        byte[] hash = pbkdf2.GetBytes(20);

        // Se comparan las passwords
        for(int i = 0; i < 20; i++)
        {
            if(hashBytes[i+16] != hash[i])
            {
                log = false;
            }
            else
            {
                log = true;
            }
        }
    }
    return log;
}
class Users
{
    public string email { get; set; }
    public string password { get; set; }
}