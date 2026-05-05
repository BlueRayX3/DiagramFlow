import { useEffect, useState } from 'react'

function App() {
  // ==========================================
  // 1. STATE'LER
  // ==========================================
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoginView, setIsLoginView] = useState(true); 
  const [currentUser, setCurrentUser] = useState(null); 

  const [authUsername, setAuthUsername] = useState('');
  const [authPassword, setAuthPassword] = useState('');
  const [authEmail, setAuthEmail] = useState('');
  const [authFirstName, setAuthFirstName] = useState('');
  const [authLastName, setAuthLastName] = useState('');

  const [projects, setProjects] = useState([]);
  const [searchQuery, setSearchQuery] = useState('');
  const [newName, setNewName] = useState('');
  const [newDesc, setNewDesc] = useState('');

  // JSON dönüşümündeki büyük/küçük harf sorununu çözen yardımcı değişkenler
  const uId = currentUser?.userID || currentUser?.UserID;
  const uName = currentUser?.username || currentUser?.Username;
  const fName = currentUser?.firstName || currentUser?.FirstName;
  const lName = currentUser?.lastName || currentUser?.LastName;

  // ==========================================
  // 2. API FONKSİYONLARI
  // ==========================================
  const fetchProjects = () => {
    fetch('https://localhost:7070/api/projects')
      .then(response => response.json())
      .then(data => setProjects(data))
      .catch(error => console.error("Bağlantı hatası:", error));
  };

  useEffect(() => {
    if (isAuthenticated) fetchProjects();
  }, [isAuthenticated]);

  const handleAddProject = (e) => {
    e.preventDefault();
    if (!newName) return;

    const newProject = { 
      Name: newName, 
      Description: newDesc,
      OwnerID: uId // Dinamik olarak giriş yapan kişinin ID'si
    };

    fetch('https://localhost:7070/api/projects', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(newProject)
    }).then(() => {
      setNewName('');
      setNewDesc('');
      fetchProjects();
    });
  };

  const handleDelete = (id) => {
    if (window.confirm("Projeyi kalıcı olarak silmek istediğinize emin misiniz?")) {
      fetch('https://localhost:7070/api/projects/' + id, {
        method: 'DELETE'
      }).then(() => fetchProjects());
    }
  };

  const handleAuthSubmit = (e) => {
    e.preventDefault();
    
    if (isLoginView) {
      fetch('https://localhost:7070/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ Username: authUsername, Password: authPassword })
      })
      .then(response => {
        if (!response.ok) throw new Error("Kullanıcı adı veya şifre hatalı!");
        return response.json();
      })
      .then(userData => {
        setCurrentUser(userData); 
        setIsAuthenticated(true); 
      })
      .catch(err => alert(err.message));

    } else {
      const registerData = {
        Username: authUsername, Password: authPassword, Email: authEmail,
        FirstName: authFirstName, LastName: authLastName
      };

      fetch('https://localhost:7070/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(registerData)
      })
      .then(response => {
        if (!response.ok) throw new Error("Kayıt olunamadı. Bilgileri kontrol edin.");
        alert("Kayıt başarılı! Şimdi giriş yapabilirsiniz.");
        setIsLoginView(true); 
      })
      .catch(err => alert(err.message));
    }
  };

  const handleLogout = () => {
    setIsAuthenticated(false);
    setCurrentUser(null);
    setAuthUsername('');
    setAuthPassword('');
  };

  const filteredProjects = projects.filter(p =>
    (p.ProjectName || p.Name || '').toLowerCase().includes(searchQuery.toLowerCase())
  );

  // ==========================================
  // 3. GÜÇLÜ CSS VE TASARIM BLOĞU
  // ==========================================
  return (
    <>
      <style>{`
        @import url('https://fonts.googleapis.com/css2?family=Plus+Jakarta+Sans:wght@400;500;600;700&display=swap');
        
        * { box-sizing: border-box; font-family: 'Plus Jakarta Sans', sans-serif; }
        body { margin: 0; background-color: #0f172a; background-image: radial-gradient(circle at 50% 0%, #1e293b 0%, #0f172a 70%); color: #f8fafc; min-height: 100vh; }
        
        /* Modern Inputlar */
        .modern-input { background: rgba(30, 41, 59, 0.5); border: 1px solid rgba(255,255,255,0.1); color: white; border-radius: 10px; padding: 14px 16px; width: 100%; outline: none; transition: all 0.3s ease; }
        .modern-input:focus { border-color: #3b82f6; box-shadow: 0 0 0 3px rgba(59, 130, 246, 0.2); background: rgba(30, 41, 59, 0.8); }
        .modern-input::placeholder { color: #64748b; }

        /* Modern Butonlar */
        .btn-primary { background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%); color: white; border: none; padding: 14px 24px; border-radius: 10px; font-weight: 600; cursor: pointer; transition: all 0.3s ease; text-shadow: 0 1px 2px rgba(0,0,0,0.2); }
        .btn-primary:hover { transform: translateY(-2px); box-shadow: 0 8px 15px rgba(59, 130, 246, 0.3); }
        .btn-danger { background: rgba(239, 68, 68, 0.1); color: #ef4444; border: 1px solid rgba(239, 68, 68, 0.2); padding: 8px 16px; border-radius: 8px; cursor: pointer; font-weight: 600; transition: all 0.2s; }
        .btn-danger:hover { background: #ef4444; color: white; }
        
        /* Kart ve Cam Efektleri */
        .glass-panel { background: rgba(30, 41, 59, 0.6); backdrop-filter: blur(12px); border: 1px solid rgba(255,255,255,0.05); border-radius: 16px; padding: 40px; width: 100%; box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.5); }
        .project-card { background: linear-gradient(180deg, rgba(30,41,59,0.8) 0%, rgba(15,23,42,0.8) 100%); border: 1px solid rgba(255,255,255,0.05); border-top: 4px solid #3b82f6; border-radius: 16px; padding: 24px; display: flex; flex-direction: column; justify-content: space-between; transition: all 0.3s ease; position: relative; overflow: hidden; }
        .project-card:hover { transform: translateY(-5px); border-top-color: #60a5fa; box-shadow: 0 20px 25px -5px rgba(0,0,0,0.3); border-left: 1px solid rgba(255,255,255,0.1); border-right: 1px solid rgba(255,255,255,0.1); }
        
        /* Scrollbar Özelleştirme */
        ::-webkit-scrollbar { width: 8px; }
        ::-webkit-scrollbar-track { background: #0f172a; }
        ::-webkit-scrollbar-thumb { background: #334155; border-radius: 4px; }
        ::-webkit-scrollbar-thumb:hover { background: #475569; }
      `}</style>

      {/* ==========================================
          A. GİRİŞ YAP / KAYIT OL EKRANI
      ========================================== */}
      {!isAuthenticated ? (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', padding: '20px' }}>
          <div className="glass-panel" style={{ maxWidth: '440px' }}>
            <div style={{ textAlign: 'center', marginBottom: '35px' }}>
              <div style={{ width: '60px', height: '60px', background: 'linear-gradient(135deg, #3b82f6, #8b5cf6)', borderRadius: '16px', margin: '0 auto 20px', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '28px' }}>
                ✨
              </div>
              <h1 style={{ margin: '0 0 10px 0', fontSize: '28px', fontWeight: '700' }}>DiagramFlow</h1>
              <p style={{ margin: 0, color: '#94a3b8' }}>
                {isLoginView ? "Çalışma alanınıza giriş yapın" : "Yeni bir çalışma alanı oluşturun"}
              </p>
            </div>

            <form onSubmit={handleAuthSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '16px' }}>
              <input type="text" className="modern-input" placeholder="Kullanıcı Adı" required value={authUsername} onChange={(e) => setAuthUsername(e.target.value)} />
              
              {!isLoginView && (
                <>
                  <input type="email" className="modern-input" placeholder="E-posta Adresi" required value={authEmail} onChange={(e) => setAuthEmail(e.target.value)} />
                  <div style={{ display: 'flex', gap: '16px' }}>
                    <input type="text" className="modern-input" placeholder="Ad" required value={authFirstName} onChange={(e) => setAuthFirstName(e.target.value)} />
                    <input type="text" className="modern-input" placeholder="Soyad" required value={authLastName} onChange={(e) => setAuthLastName(e.target.value)} />
                  </div>
                </>
              )}

              <input type="password" className="modern-input" placeholder="Şifre" required value={authPassword} onChange={(e) => setAuthPassword(e.target.value)} />
              
              <button type="submit" className="btn-primary" style={{ marginTop: '10px' }}>
                {isLoginView ? "Sisteme Giriş Yap" : "Hesabı Oluştur"}
              </button>
            </form>

            <p style={{ textAlign: 'center', color: '#64748b', marginTop: '25px', fontSize: '14px' }}>
              {isLoginView ? "Henüz hesabınız yok mu? " : "Zaten bir hesabınız var mı? "}
              <span onClick={() => setIsLoginView(!isLoginView)} style={{ color: '#60a5fa', cursor: 'pointer', fontWeight: '600' }}>
                {isLoginView ? "Hemen Kayıt Ol" : "Giriş Yap"}
              </span>
            </p>
          </div>
        </div>
      ) : (
        
        /* ==========================================
            B. ANA DASHBOARD (ÇALIŞMA ALANI)
        ========================================== */
        <div style={{ padding: '40px', maxWidth: '1400px', margin: '0 auto' }}>
          
          {/* ÜST BAR */}
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '40px', padding: '20px 30px', background: 'rgba(30, 41, 59, 0.4)', borderRadius: '16px', border: '1px solid rgba(255,255,255,0.05)' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: '15px' }}>
              <div style={{ width: '45px', height: '45px', background: 'linear-gradient(135deg, #3b82f6, #8b5cf6)', borderRadius: '12px', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '20px' }}>✨</div>
              <h1 style={{ margin: 0, fontSize: '24px', fontWeight: '700' }}>DiagramFlow <span style={{ color: '#60a5fa', fontWeight: '400' }}>Workspace</span></h1>
            </div>
            <div style={{ display: 'flex', alignItems: 'center', gap: '25px' }}>
              <div style={{ textAlign: 'right' }}>
                <div style={{ color: '#f8fafc', fontWeight: '600', fontSize: '15px' }}>{fName} {lName}</div>
                <div style={{ color: '#60a5fa', fontSize: '13px' }}>@{uName}</div>
              </div>
              <button onClick={handleLogout} className="btn-danger">Çıkış Yap</button>
            </div>
          </div>
          
          {/* ARAÇ ÇUBUĞU (Arama & Ekleme) */}
          <div style={{ display: 'flex', flexWrap: 'wrap', justifyContent: 'space-between', alignItems: 'center', marginBottom: '35px', gap: '20px', background: 'rgba(30, 41, 59, 0.4)', padding: '25px', borderRadius: '16px', border: '1px solid rgba(255,255,255,0.05)' }}>
            <input 
              type="text" 
              className="modern-input" 
              placeholder="🔍 Projelerde Ara..." 
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              style={{ maxWidth: '350px' }}
            />

            <form onSubmit={handleAddProject} style={{ display: 'flex', gap: '12px', flexWrap: 'wrap' }}>
              <input type="text" className="modern-input" placeholder="✨ Yeni Proje Adı" value={newName} onChange={(e) => setNewName(e.target.value)} style={{ width: '220px' }} />
              <input type="text" className="modern-input" placeholder="📝 Kısa Açıklama" value={newDesc} onChange={(e) => setNewDesc(e.target.value)} style={{ width: '280px' }} />
              <button type="submit" className="btn-primary" style={{ padding: '14px 28px' }}>+ Oluştur</button>
            </form>
          </div>
          
        
          
            {/* PROJE KARTLARI GRİDİ */}
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(320px, 1fr))', gap: '25px' }}>
            {filteredProjects.length === 0 ? (
              <div style={{ gridColumn: '1 / -1', textAlign: 'center', padding: '60px', color: '#64748b', fontSize: '18px', background: 'rgba(30, 41, 59, 0.3)', borderRadius: '16px', border: '1px dashed rgba(255,255,255,0.1)' }}>
                🚀 Henüz bir proje bulunamadı. Yeni bir tane oluşturarak maceraya başla!
              </div>
            ) : (
              /* BURADA p'NİN YANINA index EKLEDİK */
              filteredProjects.map((p, index) => (
                <div key={p.ProjectID} className="project-card">
                  <div>
                    <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', marginBottom: '15px' }}>
                      <h3 style={{ margin: 0, color: '#f8fafc', fontSize: '20px', fontWeight: '600' }}>{p.ProjectName || p.Name}</h3>
                      
                      {/* BURADA ID YERİNE index + 1 KULLANDIK (1, 2, 3 diye sırayla gitsin diye) */}
                      <span style={{ background: 'rgba(59, 130, 246, 0.1)', color: '#60a5fa', padding: '6px 10px', borderRadius: '8px', fontSize: '12px', fontWeight: '700', border: '1px solid rgba(59, 130, 246, 0.2)' }}>
                        Proje No: {index + 1}
                      </span>
                    </div>
                    <p style={{ color: '#94a3b8', fontSize: '15px', lineHeight: '1.6', marginBottom: '25px', minHeight: '48px' }}>
                      {p.Description || "Bu proje için bir açıklama girilmemiş."}
                    </p>
                  </div>
                  
                  <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', borderTop: '1px solid rgba(255,255,255,0.05)', paddingTop: '20px' }}>
                    <span style={{ fontSize: '13px', color: '#64748b', display: 'flex', alignItems: 'center', gap: '6px' }}>
                      <div style={{ width: '24px', height: '24px', borderRadius: '50%', background: '#3b82f6', color: 'white', display: 'flex', alignItems: 'center', justifyContent: 'center', fontWeight: 'bold', fontSize: '11px' }}>
                        {p.OwnerID}
                      </div>
                      Owner ID
                    </span>
                    <button onClick={() => handleDelete(p.ProjectID)} className="btn-danger">
                      Sil
                    </button>
                  </div>
                </div>
              ))
            )}
          </div>
        </div>
      )}
    </>
  )
}

export default App