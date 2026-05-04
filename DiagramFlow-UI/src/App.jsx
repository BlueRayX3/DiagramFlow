import { useEffect, useState } from 'react'

function App() {
  const [projects, setProjects] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  
  // Yeni proje eklemek için tuttuğumuz veriler
  const [newName, setNewName] = useState('');
  const [newDesc, setNewDesc] = useState('');

  // Verileri getiren fonksiyon
  const fetchProjects = () => {
    fetch('https://localhost:7070/api/projects')
      .then(response => response.json())
      .then(data => setProjects(data))
      .catch(error => console.error("Bağlantı hatası:", error));
  };

  // Sayfa ilk açıldığında verileri çek
  useEffect(() => {
    fetchProjects();
  }, []);

  // Yeni Proje Ekleme Fonksiyonu
  const handleAddProject = (e) => {
    e.preventDefault(); // Sayfanın yenilenmesini engeller
    if(!newName) return; // İsim boşsa işlem yapma

    const newProject = { Name: newName, Description: newDesc };

    fetch('https://localhost:7070/api/projects', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newProject)
    }).then(() => {
      setNewName(''); // Kutuları temizle
      setNewDesc('');
      fetchProjects(); // Listeyi güncelle
    });
  };

  // Proje Silme Fonksiyonu
  const handleDelete = (id) => {
    if(window.confirm("Bu projeyi silmek istediğine emin misin?")) {
      fetch('https://localhost:7070/api/projects/' + id, {
        method: 'DELETE'
      }).then(() => fetchProjects()); // Sildikten sonra listeyi güncelle
    }
  };

  // Arama filtresi (Küçük/büyük harf duyarlılığını kaldırarak arar)
  const filteredProjects = projects.filter(p => 
    p.ProjectName.toLowerCase().includes(searchQuery.toLowerCase())
  );

  return (
    <div style={{ padding: '30px', fontFamily: 'sans-serif', backgroundColor: '#f4f4f9', minHeight: '100vh' }}>
      <h1 style={{ color: '#333' }}>DiagramFlow - Aktif Projeler</h1>
      
      {/* ARAMA VE EKLEME PANELİ */}
      <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px', gap: '20px' }}>
        
        {/* Arama Kutusu */}
        <input 
          type="text" 
          placeholder="🔍 Proje Ara..." 
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.target.value)}
          style={{ padding: '10px', width: '300px', borderRadius: '5px', border: '1px solid #ccc' }}
        />

        {/* Yeni Proje Ekleme Formu */}
        <form onSubmit={handleAddProject} style={{ display: 'flex', gap: '10px' }}>
          <input 
            type="text" 
            placeholder="Proje Adı" 
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
            style={{ padding: '10px', borderRadius: '5px', border: '1px solid #ccc' }}
          />
          <input 
            type="text" 
            placeholder="Açıklama" 
            value={newDesc}
            onChange={(e) => setNewDesc(e.target.value)}
            style={{ padding: '10px', borderRadius: '5px', border: '1px solid #ccc' }}
          />
          <button type="submit" style={{ padding: '10px 20px', backgroundColor: '#28a745', color: 'white', border: 'none', borderRadius: '5px', cursor: 'pointer' }}>
            + Yeni Ekle
          </button>
        </form>
      </div>
      
      {/* TABLO */}
      <div style={{ backgroundColor: 'white', padding: '20px', borderRadius: '8px', boxShadow: '0 4px 8px rgba(0,0,0,0.1)' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
          <thead>
            <tr style={{ borderBottom: '2px solid #ddd' }}>
              <th style={{ padding: '12px' }}>ID</th>
              <th style={{ padding: '12px' }}>Proje Adı</th>
              <th style={{ padding: '12px' }}>Açıklama</th>
              <th style={{ padding: '12px' }}>Sahibi</th>
              <th style={{ padding: '12px' }}>İşlem</th>
            </tr>
          </thead>
          <tbody>
            {filteredProjects.length === 0 ? (
              <tr><td colSpan="5" style={{ padding: '12px', textAlign: 'center' }}>Proje bulunamadı...</td></tr>
            ) : (
              filteredProjects.map(p => (
                <tr key={p.ProjectID} style={{ borderBottom: '1px solid #eee' }}>
                  <td style={{ padding: '12px' }}>{p.ProjectID}</td>
                  <td style={{ padding: '12px', fontWeight: 'bold' }}>{p.ProjectName}</td>
                  <td style={{ padding: '12px' }}>{p.Description}</td>
                  <td style={{ padding: '12px', color: '#0066cc' }}>@{p.OwnerName}</td>
                  <td style={{ padding: '12px' }}>
                    <button onClick={() => handleDelete(p.ProjectID)} style={{ padding: '5px 10px', backgroundColor: '#dc3545', color: 'white', border: 'none', borderRadius: '3px', cursor: 'pointer' }}>
                      Sil
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  )
}

export default App