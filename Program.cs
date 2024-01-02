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

            warehouse.Delive(iPhone12, 10);
            warehouse.Delive(iPhone11, 1);

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
        private List<Good> _goods = new List<Good>();
        private List<int> _quantityGoods = new List<int>();

        public IReadOnlyList<Good> Goods => _goods;
        public IReadOnlyList<int> QuantityGoods => _quantityGoods;

        public virtual void Add(Good good, int amount)
        {
            bool isAvailable = false;

            for (int i = 0; i < _goods.Count; i++)
            {
                if (_goods[i].Name == good.Name)
                {
                    _quantityGoods[i] += amount;
                    isAvailable = true;
                }
            }

            if (isAvailable == false)
            {
                _goods.Add(good);
                _quantityGoods.Add(amount);
            }
        }

        private void RemoveAt(int index)
        {
            _goods.RemoveAt(index);
            _quantityGoods.RemoveAt(index);

        }

        public bool TryRequestProduct(Good good, int amount)
        {
            for (int i = 0; i < _goods.Count; i++)
            {
                if (_goods[i].Name == good.Name && _quantityGoods[i] >= amount)
                {
                    _quantityGoods[i] -= amount;

                    if (_quantityGoods[i] == 0)
                        RemoveAt(i);

                    return true;
                }
            }

            return false;
        }

        public virtual void ShowAllGoods()
        {
            for (int i = 0; i < _goods.Count; i++)
                Console.WriteLine($"Наименование: {_goods[i].Name}\tКоличество: {_quantityGoods[i]}");
        }
    }

    class Warehouse : Storage
    {
        public void Delive(Good good, int amount)
        {
            Add(good, amount);
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

            for (int i = 0; i < Goods.Count; i++)
                price += (Goods[i].Price * QuantityGoods[i]);

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
