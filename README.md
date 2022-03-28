# MiniTool

### Part of IOC

this part is implement some function of spring,it just for learning

#### Declear interface which will be scanned by our program:

```C#
   public interface IBaseService
    {
        
    }
```
#### the interface for other's invoking:

```C#
[InjectInterface]
public  interface ISomeServices:IBaseService
{       
  string getInfo();

}
```
* First in first,add ```InjectInterface``` attributte on the interface,and the programe put it into container.

* Then the interface must be implement IBaseService for it's assmbly can be scaned 

#### implement interface

```C#
 [Component]
 public class SomeService:ISomeServices
  {
  
   [AutoWired]
   private SomeRepository someRepository;/// declear SomeRepository for same way;
   
   public string getInfo()
   {
       /// someRepository.getMsg();
       return "this is a test message";
    }
  }
```

#### Declear a static factory to get obeject

```C#
public static class BLLServiceFactory 
    {
        private static  AttributeApplicationContext aacontext = null;

        public static T getService<T>(string name=null)where T:class
        {
            try
            {
                if (aacontext == null)
                {
                    var baseserviceType = typeof(IBaseService);
                    var baseRepositoryType = typeof(IBaseRepository);
                    Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    var assebmlie = assemblies.Where(p => p.GetTypes().Where(n => (baseserviceType != n && baseRepositoryType != n) && (baseserviceType.IsAssignableFrom(n) || baseserviceType.IsAssignableFrom(n))).Count() > 0).ToArray();
                    aacontext = new AttributeApplicationContext(assebmlie);
                }
                Type t = typeof(T);
                if (!typeof(IBaseService).IsAssignableFrom(t)) throw new Exception("T must implement IBaseService");
                if (!string.IsNullOrEmpty(name)) return (T)aacontext.GetBean(name);
                return aacontext.GetBean<T>();
            }
            catch (Exception ex)
            {
                LogHelper.Event += info => { };
                LogHelper.Error(ex);
            }
            return null;
        }
 ```
 
 #### we can get obeject
 ```C#
 SomeService someService = BLLServiceFactory.getService<SomeService>();
 string msg = someService.getInfo();
 ```
 
 ### Part of sigleton
 
 #### we can create a singleton object with follow code:
 
 ```C#
 public class someObject: SingleInstanceFactory<someObject>
  {
     private someObject()
     {
     }
     
     public string getMsg(){
     
      return "message form singleton"
     }
   }
   ```
   we can use it:
   
  ```C#
 public class test
 {
    public static void main(string[] args){
      string msg=someObject.current.getMsg()
    }
  }
   ```
 
 ### part of others
 ```C#
 LogHelper.Event += info => { };
 LogHelper.Error(ex);
 ```
 
 LinqExtension
 ```C#
 Expression<Func<SomeModel, bool>> conditional = x => "1".Equals(x.code);
 conditional.And(p => name.Equals(p.name));
 conditional.Or(p => message.Equals(p.message));
 ```
 CacheManager 
 ```C#
   if (!CacheManager.isSetCache("vlookup"))
   {
     CacheManager.setCache("vlookup", dic, 6);///cache 6 hours
   }
   return CacheManager.getCache<Dictionary<string, List<VLookupModel>>>("vlookup");
 ```
 
 List convert to DataTable ,DataTable convert to List etg
 
 ####  the AOP module is not tested 
 
