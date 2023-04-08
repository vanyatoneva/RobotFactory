using Newtonsoft.Json.Bson;
using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace RobotFactory.Tests
{
    public class Tests
    {
        private Factory fact;
        [SetUp]
        public void Setup()
        {
             fact = new Factory("testFFac", 10);
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test_FactoryConstructor()
        {
            Factory fact = new Factory("testFFac", 10);
            Assert.That("testFFac", Is.EqualTo(fact.Name));
            Assert.AreEqual(10, fact.Capacity);
            Assert.AreEqual(0, fact.Robots.Count);
            Assert.AreEqual(0, fact.Supplements.Count);
        }

        [Test]
        public void Test_ChangeFactoryName()
        {
            Factory fact = new Factory("testFFac", 10);
            fact.Name = "NewFact";
            Assert.AreEqual("NewFact", fact.Name);
        }

        [Test]
        public void Test_ChangeFactoryCapacity()
        {
            Factory fact = new Factory("testFFac", 10);
            fact.Capacity = 5;
            Assert.AreEqual(5, fact.Capacity);
        }

        [Test]
        public void ChangeSupplements()
        {
            List<Supplement> testSupplements = new List<Supplement>()
            {
                new Supplement("1", 1),
                new Supplement("2", 2)
            };
            fact.Supplements = testSupplements;
            Assert.AreEqual(2, fact.Supplements.Count);
            Assert.AreEqual("1", fact.Supplements[0].Name);
            Assert.AreEqual("2", fact.Supplements[1].Name);
            Assert.AreEqual(1, fact.Supplements[0].InterfaceStandard);
            Assert.AreEqual(2, fact.Supplements[1].InterfaceStandard);
        }

        [Test]
        public void ChangeRobots()
        {
            List<Robot> testRobots = new List<Robot>()
            {
                new Robot("1", 0.50, 1),
                new Robot("2", 5, 2)
            };
            fact.Robots = testRobots;
            Assert.AreEqual(2, fact.Robots.Count);
            Assert.AreEqual("1", fact.Robots[0].Model);
            Assert.AreEqual("2", fact.Robots[1].Model);
            Assert.AreEqual(1, fact.Robots[0].InterfaceStandard);
            Assert.AreEqual(2, fact.Robots[1].InterfaceStandard);
            Assert.AreEqual(0.5, fact.Robots[0].Price);
            Assert.AreEqual(5, fact.Robots[1].Price);
        }

        [Test]
        public void Test_ProduceRobots()
        {
            string result = fact.ProduceRobot("Kitchen", 150, 10000);
            Assert.AreEqual($"Produced --> Robot model: Kitchen IS: 10000, Price: {150:f2}", result);
            Assert.AreEqual(1, fact.Robots.Count);
            //TODO
            Assert.AreEqual("Kitchen", fact.Robots[0].Model);
            Assert.AreEqual(150, fact.Robots[0].Price);
            Assert.AreEqual(10000, fact.Robots[0].InterfaceStandard);
        }

        [Test]
        public void Test_ProduceRobotsWhenNoEnoughCapacity()
        {
            fact.Capacity = 1;
            fact.ProduceRobot("ththt", 200, 15000);
            string result = fact.ProduceRobot("Kitchen", 150, 10000);
            Assert.AreEqual("The factory is unable to produce more robots for this production day!", result);
            Assert.AreEqual(1, fact.Robots.Count);
            
        }

        [Test]
        public void Test_ProduceSupplement()
        {
            //Supplement sup1 = new Supplement("sup1", 1000);
            string result = fact.ProduceSupplement("sup1", 1000);
            Assert.AreEqual(1, fact.Supplements.Count);
            Assert.AreEqual("sup1", fact.Supplements[0].Name);
            Assert.AreEqual(1000, fact.Supplements[0].InterfaceStandard);
            Assert.AreEqual("Supplement: sup1 IS: 1000", result);

        }

        [Test]

        public void Test_UpgradeRobotUpgradesSuccesfully()
        {
            Supplement sup1 = new Supplement("sup1", 1000);
            //Robot robot = new Robot("rob1", 2000, 1000);
            fact.ProduceRobot("rob1", 2000, 1000);
            bool result = fact.UpgradeRobot(fact.Robots[0], sup1);
            Assert.IsTrue(result);
            Assert.AreEqual(1, fact.Robots.Count);
            Assert.AreEqual(1, fact.Robots[0].Supplements.Count);
            Assert.AreEqual("sup1", fact.Robots[0].Supplements[0].Name);
            Assert.AreEqual(1000, fact.Robots[0].Supplements[0].InterfaceStandard);
        }

        [Test]
        public void Test_UpgradeRobotWhenInterfaceStandartIsNotSupported()
        {
            //Supplement sup1 = new Supplement("sup1", 1000);
            //Robot robot = new Robot("rob1", 2000, 2000);
            fact.ProduceRobot("rob1", 2000, 2000);
            fact.ProduceSupplement("sup1", 1000);
            bool result = fact.UpgradeRobot(fact.Robots[0], fact.Supplements[0]);
            Assert.IsFalse(result);
        }



        [Test]

        public void Test_UpgradeRobotCannotAddSameSupplement()
        {
            Supplement sup1 = new Supplement("sup1", 1000);
            //Robot robot = new Robot("rob1", 2000, 1000);
            fact.ProduceRobot("rob1", 2000, 1000);
            fact.UpgradeRobot(fact.Robots[0], sup1);
            bool result = fact.UpgradeRobot(fact.Robots[0], sup1);
            Assert.IsFalse(result);
           
        }


        [Test]

        public void Test_UpgradeRobotAddDifferentSupplements()
        {
            Supplement sup1 = new Supplement("sup1", 1000);
            //Robot robot = new Robot("rob1", 2000, 1000);
            fact.ProduceRobot("rob1", 2000, 1000);
            fact.UpgradeRobot(fact.Robots[0], sup1);

            Supplement sup2 = new Supplement("sup2", 1000);
            bool result = fact.UpgradeRobot(fact.Robots[0], sup2);
            Assert.IsTrue(result);
            Assert.AreEqual(1, fact.Robots.Count);
            Assert.AreEqual(2, fact.Robots[0].Supplements.Count);
            Assert.AreEqual("sup2", fact.Robots[0].Supplements[1].Name);
            Assert.AreEqual(1000, fact.Robots[0].Supplements[1].InterfaceStandard);
        }

        [Test]
        public void Test_SellRobotSellsTheRightRobot()
        {
            fact.ProduceRobot("rob1", 2000, 5000);
            fact.ProduceRobot("rob2", 10000, 1000);
            fact.ProduceRobot("rob3", 8000, 1500);

            Robot selledRobot = fact.SellRobot(9000);
            Assert.AreEqual(3, fact.Robots.Count);
            Assert.AreEqual(8000, selledRobot.Price);
            Assert.AreEqual("rob3", selledRobot.Model);
            Assert.AreEqual(1500, selledRobot.InterfaceStandard);
        }

        [Test]
        public void Test_SellRobotsReturnsNullIfNotEnoughMoney()
        {
            fact.ProduceRobot("rob1", 2000, 5000);
            fact.ProduceRobot("rob2", 10000, 1000);
            fact.ProduceRobot("rob3", 8000, 1500);

            Robot selledRobot = fact.SellRobot(1999);
            Assert.IsNull(selledRobot);
        }



        [Test]
        public void Test_SellRobotSellsTheMostExpensiveRobot()
        {
            fact.ProduceRobot("rob1", 2000, 5000);
            fact.ProduceRobot("rob2", 10000, 1000);
            fact.ProduceRobot("rob3", 8000, 1500);

            Robot selledRobot = fact.SellRobot(10000);
            Assert.AreEqual(3, fact.Robots.Count);
            Assert.AreEqual(10000, selledRobot.Price);
            Assert.AreEqual("rob2", selledRobot.Model);
            Assert.AreEqual(1000, selledRobot.InterfaceStandard);
        }
    }
}