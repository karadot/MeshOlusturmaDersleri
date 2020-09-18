using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MeshRenderer), typeof (MeshFilter), typeof (MeshCollider))]
public class SekillendirilebilirMesh : MonoBehaviour {

    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    MeshCollider meshCollider;

    public float kareBoyutu;
    public int kareSayisi;

    void Start () {
        mesh = GetComponent<MeshFilter> ().mesh;
        meshCollider = GetComponent<MeshCollider> ();
        PlaneVerisiOlustur ();
        MeshOlustur ();
    }
    
    //Fare tıklandığı ve sürüklendiği sürece fonksiyonumuz çağırılacak
    void OnMouseDrag () {
        MeshSekillendir ();
    }
    
    //Oluşturduğumuz mesh üzerinde tıkladığımız konumlarda
    //yükseltme ve alçaltmalar yapmamızı sağlar.
    void MeshSekillendir () {
        //Fare pozisyonunu kullanarak kameradan çıkan bir ray oluşturuyoruz.
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        
        //Ray atma işlemi sonrası objemize değerse, bu veriyi tutmak için bir değişken oluşturuyoruz.
        RaycastHit hit;
        
        //Tıklama sonucu mesh üzerinde hangi üçgene değdiğimizi tutacağımız değişken.
        int triangleIndex = -1;
        
        /*
        Işını yolluyoruz. Eğer objemize değerse,
        değdiği üçgen indexini değişkenimize atıyoruz.
        Değmez ise hiçbir şey yapmadan fonksiyonu durduruyoruz
        */
        if (Physics.Raycast (ray, out hit)) {
            triangleIndex = hit.triangleIndex;
        } else {
            return;
        }
        
        //Oluşturacağımız yükseltme ve alçaltma oranı
        float eklenecekYukseklik = -.5f;
        
        /*
        Eğer sol shift tuşuna basılı tutuluyorsa
        değeri pozitife çeviriyoruz. Yani yükseltme yapacağız.
        Siz farklı bir yöntem deneyebilirsiniz,
        isterseniz daha basit yazabilirsiniz bu komutu. Size bıraktım :)
        */
        if (Input.GetKey (KeyCode.LeftShift)) {
            eklenecekYukseklik = .5f;
        }
        
        /*
        Tıkladığımız yerdeki üçgeni alıyoruz, ancak ben kare olarak hareket ettirmek istiyorum.
        Bu sebeple tıkladığımız ve bunun komşu üçgeninin sahip olduğu vertexlerin index değerlerini
        tutmak için int tipinde değişkenler oluşturdum. 
        Bu indexleri triangles dizisindenn alacağız unutmayın.
        Ayrıca ayrı ayrı yazmak yerine 6 elemanlı bir dizi de oluşturabilirsiniz.
        */
        int v0, v1, v2, v3, v4, v5;

        /*
        Ufak bir hatırlatma.
        1-3 4
        0 2-5
        Karemizi(2 adet üçgeni) oluştururken 6 vertex oluşturuyorduk.
        Unutmamanız gereken şey şu, üstte gösterdiğim şekilde 
        0.üçgenimiz 0-1-2 vertexlerinden
        1.üçgenimiz 3-4-5 vertexlerinden oluşuyordu.
        */
        
        /*
        Karelerimizi oluştururken z ekseninde ilerliyor idik. Yukarıdaki hatırlatmada da
        göreceğiniz üzere 0. üçgen altta 1.üçgen üstte bulunuyor. Devamındaki kareyi de düşünürsek 
        2.indexe sahip üçgen karenin altında, 3.indexe sahip üçgen karenin üstünde bulunuyor.
        Yani alttaki üçgen çift sayılı index'e sahipken üstteki üçgen tek sayılı indexe sahip.
        Bu çift-tek ilişkini de komşu üçgen vertexlerini bulmak için kullanacağız. 
        Eğer çift sayılı index ise komşu bir sonraki üçgen, 
        eğer tek sayılı index ise komşu bir önceki üçgen
        olacaktır.
        */ 
        
        /*
        Triangles içerisinde üçer üçer ilerlediğimizi belirtmiştim.
        Burada da aslında vertexleri bu şekilde bulacağız. 
        Aşağıdaki yapıyı da şöyle özetleyebilirim.
        triangles dizimiz = 0,1,2,3,4,5,6,7,8,9,10,11 şeklinde diyelim.
        Üçer üçer bölündüğünü tekrar hatırlarsak,
        0.üçgen: 0,1,2
        1.üçgen: 3,4,5
        2.üçgen: 6,7,8
        3.üçgen: 9,10,11 
        şeklinde olacak. 
        2. üçgenin ilk vertexini bulmak için yapacağımız şey şu
        2*3+0 yani üçgenIndexi ile 3'ü çarpıp sonrasında kaçıncı indexe
        sahip vertexi istiyorsak onu eklemek. 
        Aşağıda bu yapıyı kullandık. 
        Anlamadıysanız elinize kağıt kalemi alıp her bir üçgen ve 
        vertexlerini dediğim yol ile bulmayı deneyin.
        */
        //Eğer çift sayılı indexte isek 
        if (triangleIndex % 2 == 0) {
            //Tıkladığımız üçgen vertexleri
            v0 = triangleIndex * 3 + 0;
            v1 = triangleIndex * 3 + 1;
            v2 = triangleIndex * 3 + 2;
            //Komşu yani bir sonraki üçgen vertexleri
            v3 = (triangleIndex + 1) * 3 + 0;
            v4 = (triangleIndex + 1) * 3 + 1;
            v5 = (triangleIndex + 1) * 3 + 2;
        } 
        //Eğer tek sayılı indexte ise
        else {
            //Tıkladığımız üçgen vertexleri
            v0 = triangleIndex * 3 + 0;
            v1 = triangleIndex * 3 + 1;
            v2 = triangleIndex * 3 + 2;
            //Komşu yani bir önceki üçgen vertexleri
            v3 = (triangleIndex - 1) * 3 + 0;
            v4 = (triangleIndex - 1) * 3 + 1;
            v5 = (triangleIndex - 1) * 3 + 2;
        }
        
        /*
        Hatırlarsanız, yukarıda bulduğumuz vertex index değerleri
        aslında triangles içerisinden okuyarak bulduk.
        Orada aldığımız değer aslında vertex'in direkt index değeri değil
        o vertexin triangles dizisindeki indexi. 
        Bu yüzden de vertices içerisinde triangles[v] şeklinde yazdık.
        */
        vertices[triangles[v0]].y += eklenecekYukseklik;
        vertices[triangles[v1]].y += eklenecekYukseklik;
        vertices[triangles[v2]].y += eklenecekYukseklik;
        vertices[triangles[v3]].y += eklenecekYukseklik;
        vertices[triangles[v4]].y += eklenecekYukseklik;
        vertices[triangles[v5]].y += eklenecekYukseklik;

        MeshOlustur ();
    }
    
    //Plane(Düz Zemin) Mesh Objemizi Oluşturduğumuz Fonksiyon
    public void PlaneVerisiOlustur () {
        
        /*
        Burada kare sayısını bir fazla kullanma sebebimizi
        derslerde daha ayrıntılı anlatmıştım.
        Ancak basitçe bahsedersek,
        5x5 şeklinde bir grid oluşturursak,
        her bir satır ve sütunda 6 tane noktadan(çizgilerin kesişimi)
        olduğunu görürüz. 
        Yani kare sayımızdan 1 fazla
        */
        vertices = new Vector3[(kareSayisi + 1) * (kareSayisi + 1)];
        /*
        Her bir karemizin 6 tane vertexten oluşmasını istiyoruz.
        Bu sayede ayrık üçgenler dersimizdeki yapıyı kullanacağız.
        Triangle içerisinde de oluşturacağımız mesh yaratırken 
        oluşturduğumuz üçgenlerin noktaları veriyoruz.
        Her bir üçgen 3 noktadan oluşuyor, 
        ve biz 2 üçgeni 1 kare olarak görüyoruz.
        Yani kaç kare kullanacaksak bunun için 6 adet de
        vertex değerini triangles dizisinde tutacağız.
        Bu yüzden toplam kare sayımızı 6 ile çarpıyoruz.
        */
        triangles = new int[kareSayisi * kareSayisi * 6];
        
        /*
        Bir kareyi oluştururken vertexlerin konumunu
        kare merkezinden yatay ve dikeyde istediğimiz boyutun yarısı
        kadar olacaktır. Bu yüzden .5f ile 
        yarısını bir değişkende tutuyoruz.
        */
        float vertexAralik = kareBoyutu * .5f;
        
        //Vertexleri oluşturmak ve dolaşmak için kullanacağımız index değişkeni
        int v = 0; 
        //Triangles dizisini oluştururken kullanacağımız index değişkeni
        int t = 0; 
        
        //İçiçe for döngüsü sayesinde yatay ve dikeyde
        //vertexlerimizi oluşturuyoruz.
        for (int x = 0; x <= kareSayisi; x++) {
            for (int z = 0; z <= kareSayisi; z++) {
                /*
                X ve Z değerleri ile kare boyutunu çarparak
                oluşturacağımız karenin merkezini buluyoruz.
                Ardından da daha önce belirlediğimiz aralığı çıkararak
                yaratacağımız vertex pozisyonunu tam olarak belirlemiş oluyoruz.
                Ardından vertices dizimize bu pozisyon bilgisini kaydediyoruz.
                */
                vertices[v] = new Vector3 ((x * kareBoyutu) - vertexAralik, 0, (z * kareBoyutu) - vertexAralik);
                /*
                Her döngü sonrasında v index değişkenimizi arttırarak
                bir sonraki vertex değerine geçip bunu kaydediyoruz.
                İçiçe döngü kullanıp tek boyutlu diziye veri kaydederken
                bu yöntem basit ve kullanışlı. Farklı yöntemler de deneyebilirsiniz.
                */
                v++;
            }
        }

        v = 0;
        //Karelerin oluşturulma sırası - Mesh Görünümü
        /* 
        3 7 11 15
        2 6 10 14
        1 5 9  13
        0 4 8  12
        */
        //Karelerin dizi içerisinde görünümü
        //0 1 2 3 4 5 6 7 8 9 10 11 12 13 14 15
        
        //Bu kısımda kare sayısını içiçe for döngüsü ile
        //iki defa dönüyoruz.
        //Bu sayede kare sayısı 5 girildiyse 
        //5x5 şeklinde kare bir mesh oluşturabiliyoruz.
        for (int x = 0; x < kareSayisi; x++) {
            for (int z = 0; z < kareSayisi; z++) {
                /*
                Burada yaptığımız şey basitçe her bir kareyi dolaşmak
                ve karenin sahip olduğu 6 vertex değerini kullanarak
                2 adet üçgen oluşturmak.
                Triangles dizisindeki değerleri üçer üçer alarak
                üçgenleri oluşturduğumuzu unutmayın.
                */
                triangles[t + 0] = v;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + (kareSayisi + 1);
                triangles[t + 3] = v + (kareSayisi + 1);
                triangles[t + 4] = v + 1;
                triangles[t + 5] = v + (kareSayisi + 1) + 1;
                //Vertex değerimizi arttırarak bir sonraki vertex'e geçiyoruz.
                v++;
                /*
                Hali hazırda 6 tane vertex verisini vererek
                2 adet üçgeni oluşturduğumuz için t index değerimizi
                6 arttırıp, diğer kareye geçiyoruz.
                */
                t += 6;
            }
            /*
            Daha önceki döngüden farklı olarak burada da vertex index
            değerini arttırmamızın sebebi sınıra geliyor olmamız.
            Bahsettiğim gibi, 5 kare için 6 tane nokta oluyor yatay ve dikeyde.
            Ancak en üstteki ve sağdaki vertexlerin devamında kareler oluşturmuyoruz.
            Bu yüzden bu değeri arttırarak o vertexleri atlayıp işleme devam ediyoruz.
            */
            v++;
        }
    }
    
    /*
    Burada yaptığımız şey sırasıyla şöyle
    Mesh verisini temizliyoruz.
    Mesh'i oluşturacak vertexlerin atamasını yapıyoruz.
    Mesh'i oluşturacak üçgenlerin atamasını yapıyoruz.
    Üçgenlerin Normal değerlerini hesaplatıyoruz.
    Ve son olarak Collider bileşenimize bu oluşturduğumuz 
    mesh verisini ekliyoruz.
    */
    public void MeshOlustur () {
        mesh.Clear ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals ();
        meshCollider.sharedMesh = mesh;
    }
}
