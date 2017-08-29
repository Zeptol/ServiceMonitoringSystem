using System;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;

namespace ServiceMonitoringSystem.Web
{
    public class AutofacConfig
    {
        public static void Register()
        {
            //初始化AutoFac的相关功能  
            /* 
             1.0 告诉AutoFac初始化数据仓储层ServiceMonitoringSystem.Repository.dll中所有类的对象实例。这些对象实例以其接口的形式保存在AutoFac容器中 
             2.0 告诉AutoFac初始化业务逻辑层ServiceMonitoringSystem.Services.dll中所有类的对象实例。这些对象实例以其接口的形式保存在AutoFac容器中 
             3.0 将MVC默认的控制器工厂替换成AutoFac的工厂 
             */

            //第一步： 构造一个AutoFac的builder容器  
            ContainerBuilder builder = new ContainerBuilder();

            //第二步：告诉AutoFac控制器工厂，控制器类的创建去哪些程序集中查找（默认控制器工厂是去扫描bin目录下的所有程序集）  
            //2.1 从当前运行的bin目录下加载ServiceMonitoringSystem.Web.dll程序集  
            Assembly controllerAss = Assembly.Load("ServiceMonitoringSystem.Web");

            //2.2 告诉AutoFac控制器工厂，控制器的创建从controllerAss中查找（注意：RegisterControllers()方法是一个可变参数，如果你的控制器类的创建需要去多个程序集中查找的话，那么我们就再用Assembly controllerBss=Assembly.Load("需要的程序集名")加载需要的程序集，然后与controllerAss组成数组，然后将这个数组传递到RegisterControllers()方法中）  
            builder.RegisterControllers(controllerAss);



            //第三步：告诉AutoFac容器，创建项目中的指定类的对象实例，以接口的形式存储（其实就是创建数据仓储层与业务逻辑层这两个程序集中所有类的对象实例，然后以其接口的形式保存到AutoFac容器内存中，当然如果有需要也可以创建其他程序集的所有类的对象实例，这个只需要我们指定就可以了）  

            //3.1 加载数据仓储层ServiceMonitoringSystem.Repository这个程序集。  
            Assembly repositoryAss = Assembly.Load("ServiceMonitoringSystem.Repository");
            //3.2 反射扫描这个ServiceMonitoringSystem.Repository.dll程序集中所有的类，得到这个程序集中所有类的集合。  
            Type[] types = repositoryAss.GetTypes();
            //3.3 告诉AutoFac容器，创建rtypes这个集合中所有类的对象实例  
            foreach (var type in types)
            {
                if (type.IsGenericType)
                {
                    builder.RegisterGeneric(type).AsImplementedInterfaces();
                }
                else
                {
                    builder.RegisterType(type).AsImplementedInterfaces();
                }
            }
            //builder.RegisterTypes(types).AsImplementedInterfaces(); //指明创建的rtypes这个集合中所有类的对象实例，以其接口的形式保存  

            //3.4 加载业务逻辑层ServiceMonitoringSystem.Services这个程序集。  
            //Assembly servicesAss = Assembly.Load("ServiceMonitoringSystem.Services");
            //3.5 反射扫描这个ServiceMonitoringSystem.Services.dll程序集中所有的类，得到这个程序集中所有类的集合。  
            //Type[] stypes = servicesAss.GetTypes();
            //3.6 告诉AutoFac容器，创建stypes这个集合中所有类的对象实例  
            //builder.RegisterTypes(stypes).AsImplementedInterfaces(); //指明创建的stypes这个集合中所有类的对象实例，以其接口的形式保存  


            //第四步：创建一个真正的AutoFac的工作容器  
            var container = builder.Build();


            //我们已经创建了指定程序集的所有类的对象实例，并以其接口的形式保存在AutoFac容器内存中了。那么我们怎么去拿它呢？  
            //从AutoFac容器内部根据指定的接口获取其实现类的对象实例  
            //假设我要拿到IsysFunctionServices这个接口的实现类的对象实例，怎么拿呢？  
            //var obj = container.Resolve<ISysFunctionServices>(); //只有有特殊需求的时候可以通过这样的形式来拿。一般情况下没有必要这样来拿，因为AutoFac会自动工作（即：会自动去类的带参数的构造函数中找与容器中key一致的参数类型，并将对象注入到类中，其实就是将对象赋值给构造函数的参数）  


            //第五步：将当前容器中的控制器工厂替换掉MVC默认的控制器工厂。（即：不要MVC默认的控制器工厂了，用AutoFac容器中的控制器工厂替代）此处使用的是将AutoFac工作容器交给MVC底层 (需要using System.Web.Mvc;)  
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            //我们知道控制器的创建是调用MVC默认的控制器工厂，默认的控制器工厂是调用控制器类的无参构造函数  
            //可是我们如果要使用AutoFac自动工厂，将对象通过构造函数注入类中，那么这个构造函数就需要带参数  
            //如果我们将控制器的无参构造函数删除，保留带参数的构造函数，MVC默认的控制器工厂来创建控制的时候  
            //就会去调用无参的构造函数，可是这时候发现没有无参的构造函数于是就报“没有为该对象定义无参数的构造函数”错误  
            //既然报错,那我们如果保留无参的构造函数，同时在声明一个带参数的构造函数是否可行呢？  
            //答案;行是行，但是创建控制器的时候，MVC默认的控制器工厂调用的是无参构造函数，它并不会去调用有参的构造函数  
            //这时候，我们就只能将AutoFac它的控制器工厂替换调用MVC默认的控制器工厂（控制器由AutoFac的控制器工厂来创建）  
            //而AutoFac控制器工厂在创建控制的时候只会扫描带参数的构造函数，并将对象注入到带参数的构造函数中  
            //AutofacDependencyResolver这个控制器工厂是继承了 IDependencyResolver接口的，而IDependencyResolver接口是MVC的东西  
            //MVC默认的控制器工厂名字叫：DefaultControllerFactory  
            //具体参考：http://www.cnblogs.com/artech/archive/2012/04/01/controller-activation-032.html  

        }
    }
}