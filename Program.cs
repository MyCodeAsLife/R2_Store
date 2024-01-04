using System;
using System.Collections.Generic;

namespace R2_Store
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Good iPhone12 = new Good("IPhone 12");
            Good iPhone11 = new Good("IPhone 11");

            Warehouse warehouse = new Warehouse();

            Shop shop = new Shop(warehouse);

            warehouse.Add(iPhone12, 10);
            warehouse.Add(iPhone11, 1);

            //Вывод всех товаров на складе с их остатком
            warehouse.ShowAllGoods();

            Cart cart = shop.Cart();
            cart.Add(iPhone12, 4);
            cart.Add(iPhone11, 3); //при такой ситуации возникает ошибка так, как нет нужного количества товара на складе

            //Вывод всех товаров в корзине
            cart.ShowAllGoods();

            Console.WriteLine(cart.Order().Paylink);

            cart.Add(iPhone12, 9); //Ошибка, после заказа со склада убираются заказанные товары
        }
    }

    class Shop
    {
        private Warehouse _warehouse;

        public Shop(Warehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public Cart Cart() => new Cart(_warehouse);
    }

    abstract class Storage
    {
        private Dictionary<Good, int> _goods = new Dictionary<Good, int>();

        public IReadOnlyDictionary<Good, int> Goods => _goods;

        public virtual void Add(Good good, int amount)
        {
            bool isAvailable = false;

            foreach (var item in _goods)
            {
                if (item.Key.Name == good.Name)
                {
                    _goods[item.Key] += amount;
                    isAvailable = true;
                }
            }

            if (isAvailable == false)
                _goods.Add(good, amount);
        }

        public bool TryRequestProduct(Good good, int amount)
        {
            foreach (var item in _goods)
            {
                if (item.Key.Name == good.Name && _goods[item.Key] >= amount)
                {
                    _goods[item.Key] -= amount;

                    if (_goods[item.Key] == 0)
                        _goods.Remove(item.Key);

                    return true;
                }
            }

            return false;
        }

        public virtual void ShowAllGoods()
        {
            foreach (var item in _goods)
                Console.WriteLine($"Наименование: {item.Key.Name}\tКоличество: {item.Value}");
        }
    }

    class Warehouse : Storage
    {
        public override void Add(Good good, int amount)
        {
            base.Add(good, amount);
        }
    }

    class Cart : Storage
    {
        private Warehouse _warehouse;

        public Cart(Warehouse warehouse)
        {
            _warehouse = warehouse;
        }

        public Order Order() => new Order(OrderPrice());

        public override void Add(Good good, int amount)
        {
            if (_warehouse.TryRequestProduct(good, amount))
                base.Add(good, amount);
            else
                ShowError();
        }

        private int OrderPrice()
        {
            int price = 0;

            foreach (var item in Goods)
                price += (item.Key.Price * item.Value);

            return price;
        }

        private void ShowError()
        {
            Console.WriteLine("На складе нет указанного товара или его недостаточно.");
        }
    }

    class Good
    {
        public Good(string name)
        {
            Name = name;
            Price = 100000;
        }

        public string Name { get; private set; }
        public int Price { get; private set; }
    }

    class Order
    {
        public Order(int payLink)
        {
            Paylink = payLink;
        }

        public int Paylink { get; private set; }
    }
}
