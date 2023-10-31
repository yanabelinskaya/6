using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

public class Figure
{
    public string Name { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}

class Program
{
    static void Main(string[] args)
    {
        TextEditor textEditor = new TextEditor();
        textEditor.Start();
    }
}

public class TextEditor
{
    private List<Figure> figures = new List<Figure>();
    private string filePath = "";

    public void Start()
    {
        Console.WriteLine("Добро пожаловать в текстовый редактор!");
        Console.WriteLine("Введите путь к файлу: ");
        filePath = Console.ReadLine();

        if (File.Exists(filePath))
        {
            LoadFile();
            Console.WriteLine("Файл успешно загружен.");
            Console.WriteLine("Нажмите F1 для сохранения файла, F2 для редактирования данных, или Escape для выхода.");
        }
        else
        {
            Console.WriteLine("Файл не существует. Программа завершает работу.");
        }

        while (true)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey().Key;
                if (key == ConsoleKey.F1)
                {
                    SaveFile();
                    Console.WriteLine("Файл сохранен.");
                }
                else if (key == ConsoleKey.F2)
                {
                    EditData();
                }
                else if (key == ConsoleKey.Escape)
                {
                    Console.WriteLine("Программа завершает работу.");
                    break;
                }
            }
        }
    }

    public void LoadFile()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Путь к файлу не задан.");
            return;
        }

        try
        {
            string fileExtension = Path.GetExtension(filePath);
            string[] lines = null;

            if (fileExtension.Equals(".txt", StringComparison.OrdinalIgnoreCase))
            {
                lines = File.ReadAllLines(filePath);
            }
            else if (fileExtension.Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                string jsonContent = File.ReadAllText(filePath);
                figures = JsonConvert.DeserializeObject<List<Figure>>(jsonContent);
            }
            else if (fileExtension.Equals(".xml", StringComparison.OrdinalIgnoreCase))
            {
                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
                    figures = (List<Figure>)serializer.Deserialize(fileStream);
                }
            }
            else
            {
                Console.WriteLine("Неподдерживаемый формат файла.");
                return;
            }

            if (lines != null)
            {
                foreach (string line in lines)
                {
                    Console.WriteLine(line);
                }
            }
            else
            {
                foreach (var figure in figures)
                {
                    Console.WriteLine($"Name: {figure.Name}, Width: {figure.Width}, Height: {figure.Height}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
        }
    }

    public void SaveFile()
    {
        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Путь к файлу не задан.");
            return;
        }

        string fileExtension = Path.GetExtension(filePath).ToLower();
        string serializedData = string.Empty;

        if (fileExtension == ".txt")
        {
            serializedData = SerializeToTxt();
        }
        else if (fileExtension == ".json")
        {
            serializedData = SerializeToJson();
        }
        else if (fileExtension == ".xml")
        {
            serializedData = SerializeToXml();
        }
        else
        {
            Console.WriteLine("Неподдерживаемый формат файла.");
            return;
        }

        try
        {
            File.WriteAllText(filePath, serializedData);
            Console.WriteLine("Файл успешно сохранен.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сохранении файла: {ex.Message}");
        }
    }

    private string SerializeToTxt()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var figure in figures)
        {
            sb.AppendLine($"Name: {figure.Name}, Width: {figure.Width}, Height: {figure.Height}");
        }
        return sb.ToString();
    }

    private string SerializeToJson()
    {
        return JsonConvert.SerializeObject(figures, Newtonsoft.Json.Formatting.Indented);
    }

    private string SerializeToXml()
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Figure>));
        using (StringWriter stringWriter = new StringWriter())
        using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(xmlWriter, figures);
            return stringWriter.ToString();
        }
    }

    public void EditData()
    {
        if (figures.Count == 0)
        {
            Console.WriteLine("Нет данных для редактирования.");
            return;
        }

        Console.WriteLine("Выберите индекс элемента, который хотите отредактировать:");
        int index;
        if (int.TryParse(Console.ReadLine(), out index) && index >= 0 && index < figures.Count)
        {
            Figure figureToEdit = figures[index];

            Console.WriteLine("Введите новые данные для элемента:");
            Console.Write("Name: ");
            figureToEdit.Name = Console.ReadLine();

            Console.Write("Width: ");
            if (int.TryParse(Console.ReadLine(), out int width))
            {
                figureToEdit.Width = width;
            }

            Console.Write("Height: ");
            if (int.TryParse(Console.ReadLine(), out int height))
            {
                figureToEdit.Height = height;
            }

            Console.WriteLine("Данные успешно отредактированы.");
        }
        else
        {
            Console.WriteLine("Неверный индекс элемента.");
        }
    }
}

