using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;

public class Extractor
{
		private static uint SPI_FILE_MAGIC_BASE = 0x12FD0000;
		private static int SPECIAL_BIT = 1 << 1;
		private static int SPECIAL_BIT_MASK = ~SPECIAL_BIT;
		private byte[] fileDat = new byte[0];

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct cbddlp_file_head_t
		{
			public int magic;
			public int version;
			public float bedXmm;
			public float bedYmm;
			public float bedZmm;
			public int unknown1;
			public int unknown2;
			public int unknown3;
			public float layerHeightMilimeter;
			public float exposureTimeSeconds;
			public float exposureBottomTimeSeconds;
			public float offTimeSeconds;
			public int bottomLayers;
			public int resolutionX;
			public int resolutionY;
			public int previewOneOffsetAddress;
			public int layersDefinitionOffsetAddress;
			public int numberOfLayers;
			public int previewTwoOffsetAddress;
			public int unknown4;
			public int projectType;
			public int printParametersOffsetAddress;
			public int printParametersSize;
			public int antiAliasingLevel;
			public short lightPWM;
			public short bottomLightPWM;
			public int padding1;
			public int padding2;
			public int padding3;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct print_parameters_t
		{
			public float bottomLiftDistance;
			public float bottomLiftSpeed;
			public float liftingDistance;
			public float liftingSpeed;
			public float retractSpeed;
			public float VolumeMl;
			public float WeightG;
			public float CostDollars;
			public float BottomLightOffDelay;
			public float lightOffDelay;
			public int bottomLayerCount;
			public float P1;
			public float P2;
			public float P3;
			public float P4;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		public struct layer_definition_t
		{
			public float layerPositionZ;
			public float layerExposure;
			public float layerOffTimeSeconds;
			public int dataAddress;
			public int dataSize;
			public int unknown1;
			public int unknown2;
			public int unknown3;
			public int unknown4;
		}

		public object BytesToStruct(byte[] buf, int len, Type type)
		{
			IntPtr intPtr = Marshal.AllocHGlobal(len);
			Marshal.Copy(buf, 0, intPtr, len);
			object result = Marshal.PtrToStructure(intPtr, type);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public object BytesToStruct(byte[] buf, Type type)
		{
			return BytesToStruct(buf, buf.Length, type);
		}

		public int ExtractInfo(string InFile)
		{
			StreamReader streamReader;
			try
			{
				streamReader = new StreamReader(InFile);
			}
			catch (Exception)
			{
				System.Console.WriteLine("Unable to open CBDDLP file:" + InFile);
				return -1;
			}
			FileInfo fileInfo = new FileInfo(InFile);
			fileDat = new byte[fileInfo.Length];
			streamReader.BaseStream.Read(fileDat, 0, fileDat.Length);
			streamReader.Close();
			cbddlp_file_head_t cbddlp_file_head_t = default(cbddlp_file_head_t);
			byte[] array = new byte[Marshal.SizeOf((object)cbddlp_file_head_t)];
			Buffer.BlockCopy(fileDat, 0, array, 0, Marshal.SizeOf((object)cbddlp_file_head_t));
			cbddlp_file_head_t = (cbddlp_file_head_t)BytesToStruct(array, Marshal.SizeOf((object)cbddlp_file_head_t), cbddlp_file_head_t.GetType());
			if (cbddlp_file_head_t.magic == (SPI_FILE_MAGIC_BASE | 0x19))
			{
				System.Console.WriteLine("FILE header" + "--------");
				System.Console.WriteLine("magic: 0x{0:X}", cbddlp_file_head_t.magic);
				System.Console.WriteLine("version: " + cbddlp_file_head_t.version);
				System.Console.WriteLine("bedXmm: " + cbddlp_file_head_t.bedXmm);
				System.Console.WriteLine("bedYmm: " + cbddlp_file_head_t.bedYmm);
				System.Console.WriteLine("bedZmm: " + cbddlp_file_head_t.bedZmm);
				System.Console.WriteLine("unknown1: " + cbddlp_file_head_t.unknown1);
				System.Console.WriteLine("unknown2: " + cbddlp_file_head_t.unknown2);
				System.Console.WriteLine("unknown3: " + cbddlp_file_head_t.unknown3);
				System.Console.WriteLine("layerHeightMilimeter: " + cbddlp_file_head_t.layerHeightMilimeter);
				System.Console.WriteLine("exposureTimeSeconds: " + cbddlp_file_head_t.exposureTimeSeconds);
				System.Console.WriteLine("exposureBottomTimeSeconds: " + cbddlp_file_head_t.exposureBottomTimeSeconds);
				System.Console.WriteLine("offTimeSeconds: " + cbddlp_file_head_t.offTimeSeconds);
				System.Console.WriteLine("bottomLayers: " + cbddlp_file_head_t.bottomLayers);
				System.Console.WriteLine("resolutionX: " + cbddlp_file_head_t.resolutionX);
				System.Console.WriteLine("resolutionY: " + cbddlp_file_head_t.resolutionY);
				System.Console.WriteLine("previewOneOffsetAddress: 0x{0:X}", cbddlp_file_head_t.previewOneOffsetAddress);
				System.Console.WriteLine("layersDefinitionOffsetAddress: 0x{0:X}", cbddlp_file_head_t.layersDefinitionOffsetAddress);
				System.Console.WriteLine("numberOfLayers: " + (cbddlp_file_head_t.numberOfLayers + 1));
				System.Console.WriteLine("previewTwoOffsetAddress: 0x{0:X}", cbddlp_file_head_t.previewTwoOffsetAddress);
				System.Console.WriteLine("unknown4: " + cbddlp_file_head_t.unknown4);
				System.Console.WriteLine("projectType: " + cbddlp_file_head_t.projectType);
				if (cbddlp_file_head_t.version == 2)
				{
					System.Console.WriteLine("printParametersOffsetAddress: 0x{0:X}", cbddlp_file_head_t.printParametersOffsetAddress);
					System.Console.WriteLine("printParametersSize: " + cbddlp_file_head_t.printParametersSize);
					System.Console.WriteLine("antiAliasingLevel: " + cbddlp_file_head_t.antiAliasingLevel);
					System.Console.WriteLine("lightPWM: " + cbddlp_file_head_t.lightPWM);
					System.Console.WriteLine("bottomLightPWM: " + cbddlp_file_head_t.bottomLightPWM);
					System.Console.WriteLine("padding1: " + cbddlp_file_head_t.padding1);
					System.Console.WriteLine("padding2: " + cbddlp_file_head_t.padding2);
					System.Console.WriteLine("padding3: " + cbddlp_file_head_t.padding3);
				}
				else
				{
					System.Console.WriteLine("padding1: " + cbddlp_file_head_t.printParametersOffsetAddress);
					System.Console.WriteLine("padding2: " + cbddlp_file_head_t.printParametersSize);
					System.Console.WriteLine("padding3: " + cbddlp_file_head_t.antiAliasingLevel);
					System.Console.WriteLine("padding4: " + (cbddlp_file_head_t.lightPWM + cbddlp_file_head_t.bottomLightPWM));
				}

				if (cbddlp_file_head_t.version == 2)
				{
					System.Console.WriteLine("Print PARAMETERS" + "--------");
					print_parameters_t print_parameters_t = default(print_parameters_t);
					byte[] array2 = new byte[Marshal.SizeOf((object)print_parameters_t)];
					Buffer.BlockCopy(fileDat, cbddlp_file_head_t.printParametersOffsetAddress, array2, 0, Marshal.SizeOf((object)print_parameters_t));
					print_parameters_t = (print_parameters_t)BytesToStruct(array2, Marshal.SizeOf((object)print_parameters_t), print_parameters_t.GetType());
					System.Console.WriteLine("bottomLiftDistance: " + print_parameters_t.bottomLiftDistance);
					System.Console.WriteLine("bottomLiftSpeed: " + print_parameters_t.bottomLiftSpeed);
					System.Console.WriteLine("liftingDistance: " + print_parameters_t.liftingDistance);
					System.Console.WriteLine("liftingSpeed: " + print_parameters_t.liftingSpeed);
					System.Console.WriteLine("retractSpeed: " + print_parameters_t.retractSpeed);
					System.Console.WriteLine("Volume(ml): " + print_parameters_t.VolumeMl);
					System.Console.WriteLine("Weight(g): " + print_parameters_t.WeightG);
					System.Console.WriteLine("Cost($): " + print_parameters_t.CostDollars);
					System.Console.WriteLine("BottomLightOffDelay: " + print_parameters_t.BottomLightOffDelay);
					System.Console.WriteLine("lightOffDelay: " + print_parameters_t.lightOffDelay);
					System.Console.WriteLine("bottomLayerCount: " + print_parameters_t.bottomLayerCount);
					System.Console.WriteLine("P1: " + print_parameters_t.P1);
					System.Console.WriteLine("P2: " + print_parameters_t.P2);
					System.Console.WriteLine("P3: " + print_parameters_t.P3);
					System.Console.WriteLine("P4: " + print_parameters_t.P4);
				}
				
				System.Console.WriteLine("LAYERS" + "--------");
				int aaLevel = cbddlp_file_head_t.antiAliasingLevel;
				if (cbddlp_file_head_t.version == 1)
					aaLevel = 1;
				layer_definition_t layer_definition_t = default(layer_definition_t);
				for (int image=1; image<=aaLevel; image++)
				{
					if (aaLevel > 1) System.Console.WriteLine("Image GROUP " + image + "----");
					byte[] array3 = new byte[Marshal.SizeOf((object)layer_definition_t)];
					int layerOffset = cbddlp_file_head_t.layersDefinitionOffsetAddress;
					for (int layer=0; layer <= cbddlp_file_head_t.numberOfLayers; layer++)
					{
						Buffer.BlockCopy(fileDat, layerOffset, array3, 0, Marshal.SizeOf((object)layer_definition_t));
						layer_definition_t = (layer_definition_t)BytesToStruct(array3, Marshal.SizeOf((object)layer_definition_t), layer_definition_t.GetType());
						layerOffset +=  Marshal.SizeOf((object)layer_definition_t);
						System.Console.WriteLine("LAYER " + layer);
						System.Console.WriteLine("layerPositionZ: " + layer_definition_t.layerPositionZ);
						System.Console.WriteLine("layerExposure: " + layer_definition_t.layerExposure);
						System.Console.WriteLine("layerOffTimeSeconds: " + layer_definition_t.layerOffTimeSeconds);
						System.Console.WriteLine("dataAddress: 0x{0:X}", layer_definition_t.dataAddress);
						System.Console.WriteLine("dataSize: " + layer_definition_t.dataSize);
						System.Console.WriteLine("unknown1: " + layer_definition_t.unknown1);
						System.Console.WriteLine("unknown2: " + layer_definition_t.unknown2);
						System.Console.WriteLine("unknown3: " + layer_definition_t.unknown3);
						System.Console.WriteLine("unknown4: " + layer_definition_t.unknown4);
					}
				}
			}
			else
			{
				System.Console.WriteLine("Not a CBDDLP file!");
				return -1;
			}
		return 0;
		}
}

class MainClass
{
    static int Main(string[] args)
    {
        // Test if input arguments were supplied:
        if (args.Length < 1)
        {
            System.Console.WriteLine("Please enter CBDDLP file.");
            System.Console.WriteLine("Usage: CBDDLP <input .cbddlp file> ");
            return 1;
        }

        // Get the mask image.
        Extractor extractor = new Extractor();
        int result = extractor.ExtractInfo(args[0]);
        if (result != 0)
        {
            System.Console.WriteLine("CBDDLP file info cannot be extracted!");
            return 1;
        }
        else
            System.Console.WriteLine("Extracted info from CBDDLP file.");

        return 0;
    }
}

