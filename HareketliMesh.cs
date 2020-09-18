using UnityEngine;

[RequireComponent (typeof (MeshRenderer), typeof (MeshFilter))]
public class HareketliMesh : MonoBehaviour {

    Vector3[] vertices;
    int[] triangles;

    Mesh mesh;
    MeshCollider collider;

    public float kareBoyutu = 3;
    public int kareSayisi = 15;

    /*
    Her bir vertex'in yükseklik yani Y eksenindeki değerlerini
    tutacağımız ve hesaplayıp güncelleyeceğimiz dizi.
    */
    float[, ] yukseklik;

    //Dersten biraz farklı olarak böyle bir şey değişken de ekledim.
    //Bu değişken sayesinde PerlinNoise derecesini ayarlayabiliyoruz.
    [Range (0, 1)][SerializeField]
    float perlinScale = .75f;

    [Range (1, 15)][SerializeField]
    float maksYukseklik = 7f;

    void Start () {
        mesh = GetComponent<MeshFilter> ().mesh;
        collider = GetComponent<MeshCollider> ();
        YukseklikVerisi ();
        PlaneVerisiOlustur ();
        MeshOlustur ();
    }

    void Update () {
        YukseklikVerisi ();
        PlaneVerisiOlustur ();
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
                Ayrıca yukseklik dizisine önceden atadığımız değerleri de Y eksenine veriyoruz.
                Ardından vertices dizimize bu pozisyon bilgisini kaydediyoruz.
                */
                vertices[v] = new Vector3 ((x * kareBoyutu) - vertexAralik, yukseklik[x, z], (z * kareBoyutu) - vertexAralik);
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

    public void YukseklikVerisi () {
        //Vertex sayısının neden karesayisindan 1 fazla olduğunu PlaneVerisiOlustur
        //fonksiyonu içerisinde bahsetmiştim.
        yukseklik = new float[kareSayisi + 1, kareSayisi + 1];
        //Vertex aralik konusunda da aynı şekilde PlaneVerisiOlustur içerisinde bulabilirsiniz.
        float vertexAralik = kareBoyutu * .5f;

        //X ve Z eksenleri üzerinde ilerliyoruz.
        //2 boyutlu bir grid oluşturmuştuk sonuçta aslında.
        for (int x = 0; x <= kareSayisi; x++) {
            for (int z = 0; z <= kareSayisi; z++) {
                /*
                x ve z değerlerini kare boyutu ile çarpıp aralık değerini çıkararak
                vertex'in olacağı konumu belirliyoruz.
                Bu değerleri aslında PlaneVerisiOlustur içerisinde de tekrar hesaplıyoruz.
                Önce onu çağırıp, konumları hesaplatıp sonra yükseklik verisi hesapladığımız 
                bir yapıyı kurmayı size ödev olarak bırakıyorum. 
                Bu haliyle aynı işi 2 kere yapıyoruz sonuçta, daha optimize olmalı :)
                Ayrıca yalnızca X ekseninde hareket etmesini istediğim için, geçen zamanı da ilk değişkene ekliyorum.
                */
                float xVerisi = (x * kareBoyutu) - vertexAralik + Time.time;
                float zVerisi = (z * kareBoyutu) - vertexAralik;
                /*Yüksekliğini belirleyeceğimiz vertex'in gerçek konum değerlerini 
                PerlinNoise için değişken olarak veriyoruz.
                Bu fonksiyon bize 0 ve 1 arası bir değer verecek. maksYukseklik değeriyle de çarparak bunu
                yukseklik adlı dizimize kaydediyoruz.
                */
                float vertexYuksekligi = Mathf.PerlinNoise (xVerisi * perlinScale, zVerisi * perlinScale);
                yukseklik[x, z] = vertexYuksekligi * maksYukseklik;
            }
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
        if (mesh != null)
            mesh.Clear ();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals ();
        collider.sharedMesh = mesh;
    }

}
