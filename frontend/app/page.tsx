'use client';

import { useState } from 'react';
import { Editor } from '@monaco-editor/react';

const API_URL = 'http://localhost:5087';

export default function IDE() {
  const [code, setCode] = useState('');
  const [output, setOutput] = useState('');
  const [error, setError] = useState<string>('');

  // Crear archivo
  const handleCreateFile = () => {
    setCode('');
    setOutput('');
  };

  // Abrir archivo
  const handleOpenFile = (event: React.ChangeEvent<HTMLInputElement>) => {
    const file = event.target.files?.[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = (e) => {
        setCode(e.target?.result as string);
      };
      reader.readAsText(file);
    }
  };

  // Guardar archivo
  const handleSaveFile = () => {
    const blob = new Blob([code], { type: 'text/plain;charset=utf-8' });
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'archivo.glt';
    a.click();
    window.URL.revokeObjectURL(url);
  };

  // Ejecutar código
  const handleExecute = async () => {
    try {
      const response = await fetch(`${API_URL}/compile`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ code }),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.error || 'Error desconocido');
      }

      setOutput(data.result);
      setError('');
    } catch (err) {
      setOutput('');
      setError(err instanceof Error ? err.message : 'Error desconocido');
    }
  };
  

  return (
    <div className="min-h-screen bg-black text-[#0ff] flex flex-col font-mono">
      {/* Barra de botones (alineada) */}
      <div className="p-4 border-b border-[#ff00ff] flex justify-start items-center space-x-4">
        {/* Botón Crear */}
        <button
          onClick={handleCreateFile}
          className="yellow-button animate-slide-in"
          style={{ animationDelay: '0.1s' }}
        >
          ➕ Crear
        </button>

        {/* Botón Abrir */}
        <label
          className="yellow-button animate-slide-in"
          style={{ animationDelay: '0.2s' }}
        >
          📂 Abrir
          <input
            type="file"
            accept=".glt"
            className="hidden"
            onChange={handleOpenFile}
          />
        </label>

        {/* Botón Guardar */}
        <button
          onClick={handleSaveFile}
          className="yellow-button animate-slide-in"
          style={{ animationDelay: '0.3s' }}
        >
          💾 Guardar
        </button>

        {/* Botón de Tabla de Símbolos */}
        <button
          className="orange-button animate-slide-in"
          style={{ animationDelay: '0.5s' }}
        >
          🗂️ Tabla de Símbolos
        </button>

        {/* Botón de AST */}
        <button
          className="orange-button animate-slide-in"
          style={{ animationDelay: '0.6s' }}
        >
          🌳 AST
        </button>

        {/* Botón Ejecutar (Celeste) */}
        <button
          onClick={handleExecute}
          className="blue-button animate-slide-in"
          style={{ animationDelay: '0.7s' }}
        >
          🌟 Ejecutar
        </button>
      </div>

              {/* Panel de Errores - Ahora arriba */}
              {error && (
          <div className="error-panel">
            <div className="flex justify-between items-center">
              <h2 className="font-bold text-lg"> Error:</h2>
              <button
                onClick={() => setError('')}
                className="close-button"
              >
                ✖️
              </button>
            </div>
            <pre className="whitespace-pre-wrap break-words">{error}</pre>
          </div>
        )}

      {/* Área principal */}
      <div className="flex flex-1">
        {/* Editor */}
        <div className="w-1/2 border-r border-[#00ffcc]" style={{ boxShadow: 'inset 0 0 10px #00ffcc' }}>
          <Editor
            height="100%"
            defaultLanguage="javascript"
            theme="vs-dark"
            value={code}
            onChange={(value) => setCode(value || '')}
            options={{
              minimap: { enabled: false },
              lineNumbers: 'on',
              fontLigatures: true,
              fontSize: 16,
            }}
          />
        </div>


        {/* Consola */}
        <div className="w-1/2 p-4">
          <h2 className="text-lg font-bold mb-2 text-[#00ffcc]">
            🖥️ Consola
          </h2>
          <div
            className="bg-black border border-[#00ffcc] rounded-lg p-4 h-full overflow-auto"
            style={{ boxShadow: 'inset 0 0 20px #00ffcc' }}
          >
            <pre className="whitespace-pre-wrap break-words text-[#0ff]">
              {output}
            </pre>
          </div>



        </div>
      </div>


      {/* Estilos personalizados */}
      <style jsx>{`

      .error-panel {
        background-color: #ff3333;
        color: #fff;
        padding: 16px;
        border-radius: 8px;
        box-shadow: 0 0 15px #ff0000;
        margin-top: 16px;
        animation: slide-in 0.3s ease-out;
      }

      .close-button {
        background-color: transparent;
        color: #fff;
        font-size: 1.2rem;
        cursor: pointer;
        transition: transform 0.2s;
      }

      .close-button:hover {
        transform: scale(1.2);
        color: #ffcccc;
      }

      @keyframes slide-in {
        0% {
          opacity: 0;
          transform: translateY(-20px);
        }
        100% {
          opacity: 1;
          transform: translateY(0);
        }
      }
        .yellow-button {
          background-color: #ffcc00;
          color: #000;
          padding: 0.75rem 1.5rem;
          border-radius: 8px;
          font-weight: bold;
          box-shadow: 0 0 15px #ffcc00;
          transition: transform 0.2s, box-shadow 0.2s;
          cursor: pointer;
        }

        .yellow-button:hover {
          background-color: #ffdd55;
          transform: scale(1.05);
          box-shadow: 0 0 25px #ffdd55;
        }

        .orange-button {
          background-color: #ff6600;
          color: #FFFFFF;
          padding: 0.75rem 1.5rem;
          border-radius: 8px;
          font-weight: bold;
          box-shadow: 0 0 15px #ff6600;
          transition: transform 0.2s, box-shadow 0.2s;
          cursor: pointer;
        }

        .orange-button:hover {
          background-color: #ff8533;
          transform: scale(1.05);
          box-shadow: 0 0 25px #ff8533;
        }

        .blue-button {
          background-color: #00ffcc;
          color: #000;
          padding: 0.75rem 1.5rem;
          border-radius: 8px;
          font-weight: bold;
          box-shadow: 0 0 15px #00ffcc;
          transition: transform 0.2s, box-shadow 0.2s;
          cursor: pointer;
        }

        .blue-button:hover {
          background-color: #00ffcc;
          transform: scale(1.05);
          box-shadow: 0 0 25px #00ffcc;
        }

        @keyframes slide-in {
          0% {
            opacity: 0;
            transform: translateY(-20px);
          }
          100% {
            opacity: 1;
            transform: translateY(0);
          }
        }

        .animate-slide-in {
          animation: slide-in 0.5s ease-out forwards;
          opacity: 0;
        }
      `}</style>
    </div>
  );
}
