using UnityEngine;

namespace Study_OOP_And_Design_Pattern.Structural.Composite
{
    // Composite - 예시 A : 파일/폴더 트리
    // 상황 : 파일(개별, Leaf)과 폴더(묶음, Composite)를
    //       같은 방식으로 다룬다(크기, 계산, 출력)
    // 형태 : 잎(File)과 가지(Folder)가 같은 인터페이스.
    //       가지는 자식에게 재귀 형태로 위임을 한다

    public interface IFileNode
    {
        int Size();
        void Print(int depth);
    }
    
    public class File : IFileNode
    {
        private string name;
        private int size;

        public File(string name, int size)
        {
            this.name = name;
            this.size = size;
        }
        
        public int Size() => this.size;

        public void Print(int depth)
        {
            string empty = new string(' ', depth * 2);
            Debug.Log($"{empty} - {name} : size = {size}");
        }
    }

    public class Folder : IFileNode
    {
        public int Size()
        {
            throw new System.NotImplementedException();
        }

        public void Print(int depth)
        {
            throw new System.NotImplementedException();
        }
    }
}