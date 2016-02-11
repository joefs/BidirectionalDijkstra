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
using System.IO;


namespace CSE591PA1
{
    class PA1Solver
    {
        const bool DEBUG_ON = false;

        private enum InputMode { CLI, FILE};

        static void  Main()
        {
            bool stillRunning = true;
            while(stillRunning)
            {
                Console.WriteLine(">>>>>>>>MAIN MENU<<<<<<<<");
                Console.WriteLine();
                Console.WriteLine("What form of input would you like to use for the program?");
                Console.WriteLine("\t>Input : \'f\' for file input");
                Console.WriteLine("\t>Input : \'c\' for console input");
                Console.WriteLine("\t>Input : \'q\' to quit");
                string possible = Console.ReadLine();
                if (possible == "f" || possible == "F") Test2(InputMode.FILE);
                else if (possible == "c" || possible == "C") Test2(InputMode.CLI);
                else if (possible == "q" || possible == "Q") stillRunning = false;
                else Console.WriteLine("I'm sorry I didn't understand that. Please try again.");
            }
            Console.WriteLine();
            Console.WriteLine("Thank you. The program is now complete.");
            Console.WriteLine("Close the terminal or press return to close.");
            Console.ReadLine();

        }

        static void Test2(InputMode pIM)
        {
            string filePath = null;

            System.IO.StreamReader sR = null;
            Console.WriteLine();
            if (pIM == InputMode.CLI)
            {
                Console.WriteLine("Please enter your query in the form of:");
                Console.WriteLine("\t>: number n of cities in the graph (beginning at 0)");
                Console.WriteLine("\t>: number m of unidirectinal prexisting roads");
                Console.WriteLine("\t>: NEXT M LINES: <city1>:<city2>:<road length>");
                Console.WriteLine("\t>: number k of optional unidirectional roads");
                Console.WriteLine("\t>: NEXT K LINES: <city1>:<city2>:<road length>");
                Console.WriteLine("\t>: s (source city)");
                Console.WriteLine("\t>: t (target city)");
            }
            else
            {
                Console.WriteLine("Please enter the path of the file to pull input from.");
                filePath = Console.ReadLine();
                while (!File.Exists(filePath))
                {
                    Console.WriteLine("That file appears to not exist. Try again.");
                    filePath = Console.ReadLine();
                }
                while (IsFileinUse(filePath))
                {
                    Console.WriteLine("File is currently in use. Please close it and press enter.");
                    Console.ReadLine();
                }
                sR = new System.IO.StreamReader(filePath);
            }

            AdjacencyGraph<string, Edge<string>> graph = new AdjacencyGraph<string, Edge<string>>(true);
            // Transpose graph
            AdjacencyGraph<string, Edge<string>> tGraph = new AdjacencyGraph<string, Edge<string>>(true);
            Dictionary<Edge<string>, double> edgeCost = new Dictionary<Edge<string>, double>();
            Dictionary<Edge<string>, double> tEdgeCost = new Dictionary<Edge<string>, double>();

            int n = Convert.ToInt32((pIM == InputMode.CLI) ? Console.ReadLine() : sR.ReadLine());
            for (int i = 0; i < n; i++)
            {
                AddNodeToBoth(graph, tGraph, ""+i);
            }
            int m = Convert.ToInt32((pIM == InputMode.CLI) ? Console.ReadLine() : sR.ReadLine());
            char[] splitChars = {':'};
            string[] theParts;
            for (int i = 0; i < m; i++)
            {
                theParts = ((pIM == InputMode.CLI) ? Console.ReadLine() : sR.ReadLine()).Replace(" ", "").Replace("\t", "").Split(splitChars);
                AddEdgeToBoth(graph, tGraph, edgeCost, tEdgeCost, theParts[0], theParts[1], Convert.ToInt32(theParts[2]));
            }
            int k = Convert.ToInt32(((pIM == InputMode.CLI) ? Console.ReadLine() : sR.ReadLine()));
            Stack<string[]> optionalEdgeStack = new Stack<string[]>();
            for (int i = 0; i < k; i++)
            {
                optionalEdgeStack.Push(((pIM == InputMode.CLI) ? Console.ReadLine() : sR.ReadLine()).Replace(" ", "").Replace("\t", "").Split(splitChars));
            }
            string source = ((pIM == InputMode.CLI) ? Console.ReadLine() : sR.ReadLine());
            string target = ((pIM == InputMode.CLI) ? Console.ReadLine() : sR.ReadLine());

            System.Func<Edge<String>, double> EdgeCostFunct = (QuickGraph.Edge<string> input) => { return (edgeCost.ContainsKey(input)) ? edgeCost[input] : 1000.0; };
            System.Func<Edge<String>, double> EdgeCostFunct2 = (QuickGraph.Edge<string> input) => { return (tEdgeCost.ContainsKey(input)) ? tEdgeCost[input] : 1000.0; };



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
            DijkstraShortestPathAlgorithm<string, Edge<string>> dijkstra2 = new DijkstraShortestPathAlgorithm<string, Edge<string>>(tGraph, EdgeCostFunct2);

            // attach a distance observer to give us the shortest path distances

            VertexDistanceRecorderObserver<string, Edge<string>> distObserver2 = new VertexDistanceRecorderObserver<string, Edge<string>>(EdgeCostFunct2);
            distObserver2.Attach(dijkstra2);

            // Attach a Vertex Predecessor Recorder Observer to give us the paths
            VertexPredecessorRecorderObserver<string, Edge<string>> predecessorObserver2 = new VertexPredecessorRecorderObserver<string, Edge<string>>();
            predecessorObserver2.Attach(dijkstra2);





            // Run the algorithm with starname set to be the source
            dijkstra.Compute(source);


            if (distObserver.Distances.ContainsKey(target) == false)
            {
                Console.WriteLine(target + " is unreachable");
            }
            else
            {
                dijkstra2.Compute(target);

                double initialPathLength = distObserver.Distances[target];
                Stack<string> viablePathAdditions = new Stack<string>();
                string currentMinEdgeAddition = "";
                double currentMinEdgeWeight = -1.0;
                while (optionalEdgeStack.Count > 0)
                {
                    string[] triple = optionalEdgeStack.Pop();
                    if (distObserver.Distances.ContainsKey(triple[0]) && distObserver2.Distances.ContainsKey(triple[1]))
                    {
                        double total = distObserver.Distances[triple[0]] + distObserver2.Distances[triple[1]] + (double)Int32.Parse(triple[2]);
                        if (total < initialPathLength)
                        {
                            viablePathAdditions.Push(triple[0] + ':' + triple[1]);
                            if (currentMinEdgeWeight < 0 || total < currentMinEdgeWeight)
                            {
                                currentMinEdgeWeight = total;
                                currentMinEdgeAddition = triple[0] + ':' + triple[1];
                            }
                        }
                    }
                }
                if (viablePathAdditions.Count > 0)
                {
                    Console.WriteLine("Additions that would lower path length.");
                    while (viablePathAdditions.Count > 0)
                    {
                        Console.WriteLine(viablePathAdditions.Pop());
                    }
                    Console.WriteLine("The cost-minimizing addition is:");
                    Console.WriteLine(currentMinEdgeAddition);
                }
                else
                {
                    Console.WriteLine("There are no additions that would minimize path length.");
                }
            }
            Console.WriteLine("");
            Console.WriteLine("Press enter to return to the main menu.");
            Console.ReadLine();
            if (sR != null) sR.Close();
        }

        //graph, tgraph, node

        public static void AddNodeToBoth(AdjacencyGraph<string, Edge<string>> graph, AdjacencyGraph<string, Edge<string>> tGraph, string nodeString)
        {
            graph.AddVertex(nodeString);
            tGraph.AddVertex(nodeString);
        }

        //graph, tgraph, sourceNode, endNode, weight

        public static void AddEdgeToBoth(AdjacencyGraph<string, Edge<string>> graph, AdjacencyGraph<string, Edge<string>> tGraph, Dictionary<Edge<string>, double> edgeCost, Dictionary<Edge<string>, double> tEdgeCost, string sourceString, string endString, int weight)
        {
            Edge<string> curEdge = new Edge<string>(sourceString, endString);
            graph.AddEdge(curEdge);
            edgeCost.Add(curEdge, weight);


            Edge<string> transposeEdge = new Edge<string>(endString, sourceString);
            tGraph.AddEdge(transposeEdge);
            tEdgeCost.Add(transposeEdge, weight);
        }




        static bool IsFileinUse(string filePath)
        {
            FileInfo file = new FileInfo(filePath);
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return false;
        }



    }
}
