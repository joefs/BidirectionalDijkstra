using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Search;
using QuickGraph.Algorithms.ShortestPath;
using QuickGraph.Algorithms.Observers;
//using QuickGraph.Data;


namespace CSE591PA1
{
    class PA1Solver
    {
        const bool DEBUG_ON = false; 
        //[System.Runtime.InteropServices.DllImport(@"QuickGraph.dll")]
        static void  Main()
        {
            AdjacencyGraph<string, Edge<string>> graph = new AdjacencyGraph<string, Edge<string>>(true);
            // Transpose graph
            AdjacencyGraph<string, Edge<string>> tGraph = new AdjacencyGraph<string, Edge<string>>(true);
            Dictionary<Edge<string>, double> edgeCost = new Dictionary<Edge<string>, double>();

            //graph, tgraph, node

            AddNodeToBoth(graph, tGraph, "A");
            AddNodeToBoth(graph, tGraph, "B");
            AddNodeToBoth(graph, tGraph, "C");
            AddNodeToBoth(graph, tGraph, "D");
            AddNodeToBoth(graph, tGraph, "E");
            AddNodeToBoth(graph, tGraph, "F");
            AddNodeToBoth(graph, tGraph, "G");
            AddNodeToBoth(graph, tGraph, "H");
            AddNodeToBoth(graph, tGraph, "I");
            AddNodeToBoth(graph, tGraph, "J");
            AddNodeToBoth(graph, tGraph, "Y");
            AddNodeToBoth(graph, tGraph, "Z");
            //graph, tgraph, sourceNode, endNode, weight
            AddEdgeToBoth(graph, tGraph, edgeCost, "A", "B", 4);
            AddEdgeToBoth(graph, tGraph, edgeCost, "A", "D", 1);
            AddEdgeToBoth(graph, tGraph, edgeCost, "B", "A",74);
            AddEdgeToBoth(graph, tGraph, edgeCost, "B", "C", 2);
            AddEdgeToBoth(graph, tGraph, edgeCost, "B", "E", 12);
            AddEdgeToBoth(graph, tGraph, edgeCost, "C", "B",12);
            AddEdgeToBoth(graph, tGraph, edgeCost, "C", "F",74);
            AddEdgeToBoth(graph, tGraph, edgeCost, "C", "J",12);
            AddEdgeToBoth(graph, tGraph, edgeCost, "D", "E",32);
            AddEdgeToBoth(graph, tGraph, edgeCost, "D", "G", 22);
            AddEdgeToBoth(graph, tGraph, edgeCost, "E", "D",66);
            AddEdgeToBoth(graph, tGraph, edgeCost, "E", "F",76);
            AddEdgeToBoth(graph, tGraph, edgeCost, "E", "H",33);
            AddEdgeToBoth(graph, tGraph, edgeCost, "F", "I",11);
            AddEdgeToBoth(graph, tGraph, edgeCost, "F", "J",21);
            AddEdgeToBoth(graph, tGraph, edgeCost, "G", "D",12);
            AddEdgeToBoth(graph, tGraph, edgeCost, "G", "H",10);
            AddEdgeToBoth(graph, tGraph, edgeCost, "H", "G",2);
            AddEdgeToBoth(graph, tGraph, edgeCost, "H", "I",72);
            AddEdgeToBoth(graph, tGraph, edgeCost, "I", "F",31);
            AddEdgeToBoth(graph, tGraph, edgeCost, "I", "J",18);
            AddEdgeToBoth(graph, tGraph, edgeCost, "I", "H",7);
            AddEdgeToBoth(graph, tGraph, edgeCost, "J", "F",8);
            AddEdgeToBoth(graph, tGraph, edgeCost, "Y", "Z",1);

            /*
             */




            System.Func< Edge<String>, double > EdgeCostFunct = (QuickGraph.Edge<string> input) => { return (edgeCost.ContainsKey(input)) ? edgeCost[input] : 0.0; };


            //FORWARD SEARCH


            // We want to use Dijkstra on this graph
            DijkstraShortestPathAlgorithm<string, Edge<string>> dijkstra = new DijkstraShortestPathAlgorithm<string, Edge<string>>(graph, EdgeCostFunct);

            // attach a distance observer to give us the shortest path distances

            VertexDistanceRecorderObserver<string, Edge<string>> distObserver = new VertexDistanceRecorderObserver<string, Edge<string>>(EdgeCostFunct);
            distObserver.Attach(dijkstra);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            VertexPredecessorRecorderObserver<string, Edge<string>> predecessorObserver = new VertexPredecessorRecorderObserver<string, Edge<string>>();
            predecessorObserver.Attach(dijkstra);


            //BACKWARD SEARCH


            // We want to use Dijkstra on this graph
            DijkstraShortestPathAlgorithm<string, Edge<string>> dijkstra2 = new DijkstraShortestPathAlgorithm<string, Edge<string>>(tGraph, EdgeCostFunct);

            // attach a distance observer to give us the shortest path distances

            VertexDistanceRecorderObserver<string, Edge<string>> distObserver2 = new VertexDistanceRecorderObserver<string, Edge<string>>(EdgeCostFunct);
            distObserver2.Attach(dijkstra2);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            VertexPredecessorRecorderObserver<string, Edge<string>> predecessorObserver2 = new VertexPredecessorRecorderObserver<string, Edge<string>>();
            predecessorObserver2.Attach(dijkstra2);



            string startName = null;
            while(startName == null)
            {
                Console.WriteLine("What is the start point?");
                string name = Console.ReadLine();
                if (graph.ContainsVertex(name))
                {
                    startName = name;
                }
                else
                {
                    Console.Write("Not contained. Try another.");
                }
            }



            string targetName = null;
            while (targetName == null)
            {
                Console.WriteLine("What is the target point?");
                string name = Console.ReadLine();
                if (graph.ContainsVertex(name))
                {
                    targetName = name;
                }
                else
                {
                    Console.Write("Not contained. Try another.");
                }
            }


            // Run the algorithm with starname set to be the source
            dijkstra.Compute(startName);


            if (distObserver.Distances.ContainsKey(targetName) == false)
            {
                Console.WriteLine(targetName + " is unreachable");
            }
            else
            {
                if (DEBUG_ON)
                {

                    foreach (KeyValuePair<string, double> kvp in distObserver.Distances)
                        Console.WriteLine("Distance from root to node {0} is {1}", kvp.Key, kvp.Value);
                    foreach (KeyValuePair<string, Edge<string>> kvp in predecessorObserver.VertexPredecessors)
                        Console.WriteLine("If you want to get to {0} you have to enter through the in edge {1}", kvp.Key, kvp.Value);
                }
                // Run it backwarss from the goal till the parent is itself

                //Current node add to stack, find predecessor, add to stack, distance is 0

                Stack<string> nodeStringStack = new Stack<string>();
                string currentNodeString = targetName;

                while (distObserver.Distances.ContainsKey(currentNodeString) && distObserver.Distances[currentNodeString] > 0)
                {
                    nodeStringStack.Push("" + currentNodeString);
                    if(DEBUG_ON)Console.WriteLine("currentNodeString Node Being Pushed is " + currentNodeString);
                    currentNodeString = "" + predecessorObserver.VertexPredecessors[currentNodeString].Source;
                }
                if (distObserver.Distances.ContainsKey(currentNodeString) && distObserver.Distances[currentNodeString] == 0)
                {
                    nodeStringStack.Push("" + currentNodeString);
                    if (DEBUG_ON) Console.WriteLine("currentNodeString Node Being Pushed is " + currentNodeString);
                }
                Console.WriteLine("Order derived from forward search.");
                while (nodeStringStack.Count > 0)
                {
                    Console.WriteLine(nodeStringStack.Pop());
                }



                dijkstra2.Compute(targetName);
                if (DEBUG_ON)
                {
                    foreach (KeyValuePair<string, double> kvp in distObserver2.Distances)
                        Console.WriteLine("Distance from root to node {0} is {1}", kvp.Key, kvp.Value);
                    foreach (KeyValuePair<string, Edge<string>> kvp in predecessorObserver2.VertexPredecessors)
                        Console.WriteLine("If you want to get to {0} you have to enter through the in edge {1}", kvp.Key, kvp.Value);
                }


                Console.WriteLine("Order derived from backward search on transpose graph.");
                //loop through and print out
                currentNodeString = startName;
                while (distObserver2.Distances.ContainsKey(currentNodeString) && distObserver2.Distances[currentNodeString] > 0)
                {
                    Console.WriteLine("" + currentNodeString);
                    currentNodeString = "" + predecessorObserver2.VertexPredecessors[currentNodeString].Source;
                }
                if (distObserver2.Distances.ContainsKey(currentNodeString) && distObserver2.Distances[currentNodeString] == 0)
                {
                    Console.WriteLine("" + currentNodeString);
                }


                // Remember to detach the observers
                //distObserver.Detach(dijkstra);
                //predecessorObserver.Detach(dijkstra);

            }
            Console.ReadLine();
            }

        //graph, tgraph, node

        public static void AddNodeToBoth(AdjacencyGraph<string, Edge<string>> graph, AdjacencyGraph<string, Edge<string>> tGraph, string nodeString)
        {
            graph.AddVertex(nodeString);
            tGraph.AddVertex(nodeString);
        }

        //graph, tgraph, sourceNode, endNode, weight

        public static void AddEdgeToBoth(AdjacencyGraph<string, Edge<string>> graph, AdjacencyGraph<string, Edge<string>> tGraph, Dictionary<Edge<string>, double> edgeCost,string sourceString, string endString, int weight)
        {
            Edge<string> curEdge = new Edge<string>(sourceString, endString);
            graph.AddEdge(curEdge);
            edgeCost.Add(curEdge, weight);


            Edge<string> transposeEdge = new Edge<string>(endString, sourceString);
            tGraph.AddEdge(transposeEdge);
            edgeCost.Add(transposeEdge, weight);
        }

    }
}
