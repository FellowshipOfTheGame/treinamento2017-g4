using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probability //classe que administra as probabilidades baseando-se em numeros
{
    //public

    //private
    private int quantidade; //quantidade de possibilidades (sem contar a nula)
    private int[] probabilidades; //probabilidades de cada item

    private int[] probabilityVector; //vetor de probabilidades

    public Probability(int quantidade, int[] probabilidades) //construtor
    {
        this.quantidade = quantidade;
        this.probabilidades = probabilidades;

        if (!VerificarCorretude()) Debug.Log("Erro! Favor recriar probabilidades.");
        else ProbabilityVector_Preencher();
    }

    private bool VerificarCorretude() //verifica se da pra usar como probabilidade pra 100
    {
        int aux = 0;
        foreach (int i in probabilidades)
        {
            aux += i;
        }

        if (quantidade < 0) return false;
        if (aux >= 0 && aux <= 100) return true;

        return false;
    }

    private void ProbabilityVector_Preencher() //preenche vetor de probabilidades para escolha
    {
        probabilityVector = new int[100]; //vetor de 100 posicoes

        int auxDifference = 0; //diferenca de probabilidades
        for (int i = 0; i < quantidade; i++)
        {
            int j = 0;
            for (; j < probabilidades[i]; j++)
            {
                probabilityVector[j + auxDifference] = i;
            }
            auxDifference += j;
        }

        for (int j = 0; (j + auxDifference) < 100; j++)
        {
            probabilityVector[j + auxDifference] = -1; //preencher probabilidade de null/nada = resto
        }
    }

    public int ProbabilityVector_ChooseOne() //retorna uma posicao aleatoria
    {
        return probabilityVector[Random.Range(0, 100)]; //escolher posicao aleatoria entre 0 (inc) e 100 (exc) --> 0 <= x < 100
    }

    public void ProbabilityVector_Imprimir() //imprimir vetor de probabilidades
    {
        Debug.Log("Probability Vector >>\n");
        for (int i = 0; i < 100; i++)
        {
            Debug.Log(i + ": " + probabilityVector[i] + "\n");
        }
    }
}
