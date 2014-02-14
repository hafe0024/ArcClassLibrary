using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enbridge.Examples
{
    /// <summary>
    /// Abstract class Animal
    /// </summary>
    public abstract class Animal
    {
        /// <summary>
        /// 
        /// </summary>
        public int? num_legs = null;
        /// <summary>
        /// 
        /// </summary>
        public bool is_awake;
        /// <summary>
        /// 
        /// </summary>
        public string color;

        /// <summary>
        /// Base constructor for Animal
        /// </summary>
        /// <param name="color"></param>
        public Animal(string color)
        {
            this.color = color;
            this.is_awake = true;
        }

        /// <summary>
        /// eat method, to be inherited by all derived classes
        /// </summary>
        public void eat()
        {
            Console.WriteLine("The {0} is eating something", this.GetType());
        }
    }

    /// <summary>
    /// Define an interface
    /// </summary>
    public interface IAnimalActions
    {
        /// <summary>
        /// 
        /// </summary>
        void talk();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        void move(int distance);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyNumber"></param>
        /// <returns></returns>
        string getColor(int anyNumber);
    }

    /// <summary>
    /// 
    /// </summary>
    public class Bird : Animal, IAnimalActions
    {
        /// <summary>
        /// 
        /// </summary>
        public bool hasFeathers = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public Bird(string color)
            : base(color)
        {
            this.num_legs = 2;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string stringRepresentationOfBird;
            stringRepresentationOfBird = "Bird info\n\t";
            stringRepresentationOfBird += string.Format("Legs: {0}, Color: {1}", this.num_legs, this.color);
            return stringRepresentationOfBird;
        }

        /// <summary>
        /// 
        /// </summary>
        public void talk()
        {
            Console.WriteLine("The {0} bird says tweet", this.color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public void move(int distance)
        {
            Console.WriteLine("The {0} bird flew {1} feet", this.color, distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyNumber"></param>
        /// <returns></returns>
        public string getColor(int anyNumber)
        {
            return this.color;
        }


    }

    /// <summary>
    /// 
    /// </summary>
    public class Cat : Animal, IAnimalActions, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        public bool isFurry = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        public Cat(string color)
            : base(color)
        {
            this.num_legs = 4;
        }

        /// <summary>
        /// 
        /// </summary>
        public void talk()
        {
            Console.WriteLine("The {0} cat says meow", this.color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        public void move(int distance)
        {
            Console.WriteLine("The {0} cat ran {1} feet", this.color, distance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="anyNumber"></param>
        /// <returns></returns>
        public string getColor(int anyNumber)
        {
            return this.color;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.is_awake = false;
            Console.WriteLine("Run dispose");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class SomeStaticMethods
    {
        /// <summary>
        /// Check if the animal is awake
        /// </summary>
        /// <param name="one_animal"></param>
        /// <returns>boolean if is awake</returns>
        public static bool isTheAnimalAwake(Animal one_animal)
        {
            return one_animal.is_awake;
        }

        /// <summary>
        /// Random writeline stuff
        /// </summary>
        public static void writeSomeStuff()
        {
            Console.WriteLine("I'm in a static method");
        }

        /// <summary>
        /// invoke dispose on any object implementing IDisposable
        /// </summary>
        /// <param name="any_object_implementing_IDisposable"></param>
        public static void run_dispose(IDisposable any_object_implementing_IDisposable)
        {
            any_object_implementing_IDisposable.Dispose();
        }

    }
}
