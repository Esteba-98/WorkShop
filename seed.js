const https = require('https');

const BASE = 'https://workshop-api-cp3o.onrender.com/api';
const TOKEN = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjAxOWRkMjM5LTY5OWEtNzA5My1iMTA3LTRmMjMwOTQ0Njk2YiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWUiOiJlc3RlYmFuY29sb3JhZG85ODA3QGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6ImVzdGViYW5jb2xvcmFkbzk4MDdAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW5pc3RyYWRvciIsImV4cCI6MTc3NzM1MjY2NiwiaXNzIjoiV29ya3Nob3BBcGkiLCJhdWQiOiJXb3Jrc2hvcEFwaVVzZXJzIn0.TLbdQrodRfV9QDMyZCowPLjkgFXpjnhjG9l02yzEB3A';

function request(method, path, body, useAuth = true) {
  return new Promise((resolve, reject) => {
    const url = new URL(BASE + path);
    const data = body ? JSON.stringify(body) : null;
    const options = {
      hostname: url.hostname,
      path: url.pathname,
      method,
      headers: {
        'Content-Type': 'application/json',
        ...(useAuth ? { 'Authorization': `Bearer ${TOKEN}` } : {}),
        ...(data ? { 'Content-Length': Buffer.byteLength(data) } : {})
      }
    };
    const req = https.request(options, res => {
      let raw = '';
      res.on('data', c => raw += c);
      res.on('end', () => {
        try { resolve(JSON.parse(raw)); }
        catch { resolve(raw); }
      });
    });
    req.on('error', reject);
    if (data) req.write(data);
    req.end();
  });
}

async function main() {
  console.log('=== Creando usuarios ===');
  const usuarios = [
    { email: 'juan.mecanico@workshop.com', password: 'Millos_2026*', nombre: 'Juan Pérez', role: 'Mecanico' },
    { email: 'carlos.mecanico@workshop.com', password: 'Millos_2026*', nombre: 'Carlos Rodríguez', role: 'Mecanico' },
    { email: 'miguel.mecanico@workshop.com', password: 'Millos_2026*', nombre: 'Miguel Torres', role: 'Mecanico' },
    { email: 'maria.secretaria@workshop.com', password: 'Millos_2026*', nombre: 'María González', role: 'Secretaria' },
    { email: 'pedro.almacen@workshop.com', password: 'Millos_2026*', nombre: 'Pedro Almacén', role: 'OperarioAlmacen' },
  ];
  for (const u of usuarios) {
    const r = await request('POST', '/auth/register', u, false);
    console.log(`  ${u.nombre} (${u.role}):`, r.message || r);
  }

  // Get mechanic IDs
  console.log('\n=== Obteniendo IDs de mecánicos ===');
  const usersRes = await request('GET', '/Users');
  const mecanicos = Array.isArray(usersRes) ? usersRes.filter(u => u.rol === 'Mecanico') : [];
  console.log('Mecánicos encontrados:', mecanicos.map(m => `${m.nombre} - ${m.id}`));

  console.log('\n=== Creando clientes y vehículos ===');
  const clientesData = [
    { nombre: 'Carlos Martínez', email: 'carlos.martinez@gmail.com', telefono: '3001234567', marca: 'Toyota', modelo: 'Corolla', placa: 'ABC-123', anio: 2019 },
    { nombre: 'Ana López', email: 'ana.lopez@gmail.com', telefono: '3012345678', marca: 'Chevrolet', modelo: 'Spark', placa: 'DEF-456', anio: 2020 },
    { nombre: 'Luis García', email: 'luis.garcia@gmail.com', telefono: '3023456789', marca: 'Renault', modelo: 'Logan', placa: 'GHI-789', anio: 2018 },
    { nombre: 'Sofia Hernández', email: 'sofia.hernandez@gmail.com', telefono: '3034567890', marca: 'Mazda', modelo: 'CX-5', placa: 'JKL-012', anio: 2021 },
    { nombre: 'Roberto Díaz', email: 'roberto.diaz@gmail.com', telefono: '3045678901', marca: 'Nissan', modelo: 'Sentra', placa: 'MNO-345', anio: 2017 },
    { nombre: 'Valentina Ruiz', email: 'valentina.ruiz@gmail.com', telefono: '3056789012', marca: 'Kia', modelo: 'Picanto', placa: 'PQR-678', anio: 2022 },
    { nombre: 'Andrés Torres', email: 'andres.torres@gmail.com', telefono: '3067890123', marca: 'Hyundai', modelo: 'Tucson', placa: 'STU-901', anio: 2020 },
    { nombre: 'Camila Vargas', email: 'camila.vargas@gmail.com', telefono: '3078901234', marca: 'Ford', modelo: 'EcoSport', placa: 'VWX-234', anio: 2019 },
    { nombre: 'Diego Morales', email: 'diego.morales@gmail.com', telefono: '3089012345', marca: 'Volkswagen', modelo: 'Polo', placa: 'YZA-567', anio: 2021 },
    { nombre: 'Paola Jiménez', email: 'paola.jimenez@gmail.com', telefono: '3090123456', marca: 'Suzuki', modelo: 'Swift', placa: 'BCD-890', anio: 2018 },
  ];

  const clienteIds = [];
  const vehiculoIds = [];
  for (const c of clientesData) {
    const cliente = await request('POST', '/Cliente', { nombre: c.nombre, email: c.email, telefono: c.telefono });
    console.log(`  Cliente: ${c.nombre} -> ${cliente.id}`);
    const vehiculo = await request('POST', '/Vehiculos', { clienteId: cliente.id, placa: c.placa, marca: c.marca, modelo: c.modelo, anio: c.anio });
    console.log(`  Vehiculo: ${c.marca} ${c.modelo} -> ${vehiculo.id}`);
    clienteIds.push(cliente.id);
    vehiculoIds.push(vehiculo.id);
  }

  console.log('\n=== Creando productos ===');
  const productosData = [
    { codigo: 'REP-001', nombre: 'Filtro de aceite', precio: 25000, stock: 50 },
    { codigo: 'REP-002', nombre: 'Pastillas de freno delanteras', precio: 85000, stock: 30 },
    { codigo: 'REP-003', nombre: 'Aceite motor 5W-30 (1L)', precio: 18000, stock: 100 },
    { codigo: 'REP-004', nombre: 'Filtro de aire', precio: 32000, stock: 40 },
    { codigo: 'REP-005', nombre: 'Bujías (set x4)', precio: 45000, stock: 35 },
    { codigo: 'REP-006', nombre: 'Correa de distribución', precio: 120000, stock: 20 },
    { codigo: 'REP-007', nombre: 'Amortiguador delantero', precio: 180000, stock: 15 },
    { codigo: 'REP-008', nombre: 'Batería 60Ah', precio: 320000, stock: 10 },
    { codigo: 'REP-009', nombre: 'Bomba de agua', precio: 95000, stock: 25 },
    { codigo: 'REP-010', nombre: 'Termostato', precio: 28000, stock: 30 },
    { codigo: 'REP-011', nombre: 'Líquido de frenos DOT4', precio: 15000, stock: 60 },
    { codigo: 'REP-012', nombre: 'Anticongelante (1L)', precio: 12000, stock: 80 },
  ];
  const productoIds = [];
  for (const p of productosData) {
    const prod = await request('POST', '/Productos', p);
    console.log(`  Producto: ${p.nombre} -> ${prod.id}`);
    productoIds.push(prod.id);
  }

  console.log('\n=== Creando órdenes de mantenimiento ===');
  const ordenes = [
    { ci: 0, vi: 0, desc: 'Cambio de aceite y filtros', diag: 'Motor con ruido leve, se recomienda cambio preventivo', mecanico: 0,
      items: [{ tipo: 'Producto', productoId: 0, cantidad: 4 }, { tipo: 'Producto', productoId: 2, cantidad: 1 }, { tipo: 'Servicio', nombre: 'Mano de obra cambio aceite', precioUnitario: 30000, cantidad: 1 }] },
    { ci: 1, vi: 1, desc: 'Revisión de frenos', diag: 'Pastillas desgastadas al 20%, disco con rayaduras leves', mecanico: 1,
      items: [{ tipo: 'Producto', productoId: 1, cantidad: 1 }, { tipo: 'Servicio', nombre: 'Mano de obra frenos', precioUnitario: 50000, cantidad: 1 }] },
    { ci: 2, vi: 2, desc: 'Mantenimiento preventivo 30.000 km', diag: 'Vehículo en buen estado general, mantenimiento de rutina', mecanico: 2,
      items: [{ tipo: 'Producto', productoId: 0, cantidad: 1 }, { tipo: 'Producto', productoId: 3, cantidad: 1 }, { tipo: 'Producto', productoId: 4, cantidad: 1 }, { tipo: 'Servicio', nombre: 'Revisión general', precioUnitario: 80000, cantidad: 1 }] },
    { ci: 3, vi: 3, desc: 'Cambio de correa de distribución', diag: 'Correa con más de 60.000 km, riesgo de rotura', mecanico: 0,
      items: [{ tipo: 'Producto', productoId: 5, cantidad: 1 }, { tipo: 'Servicio', nombre: 'Mano de obra correa distribución', precioUnitario: 120000, cantidad: 1 }] },
    { ci: 4, vi: 4, desc: 'Reemplazo de batería', diag: 'Batería no carga correctamente, voltaje bajo', mecanico: 1,
      items: [{ tipo: 'Producto', productoId: 7, cantidad: 1 }, { tipo: 'Servicio', nombre: 'Instalación batería', precioUnitario: 20000, cantidad: 1 }] },
    { ci: 5, vi: 5, desc: 'Revisión suspensión delantera', diag: 'Amortiguadores delanteros con fugas de aceite', mecanico: 2,
      items: [{ tipo: 'Producto', productoId: 6, cantidad: 2 }, { tipo: 'Servicio', nombre: 'Mano de obra suspensión', precioUnitario: 90000, cantidad: 1 }] },
    { ci: 6, vi: 6, desc: 'Diagnóstico sistema de refrigeración', diag: 'Temperatura motor elevada, posible falla en bomba de agua', mecanico: 0,
      items: [{ tipo: 'Producto', productoId: 8, cantidad: 1 }, { tipo: 'Producto', productoId: 11, cantidad: 2 }, { tipo: 'Servicio', nombre: 'Mano de obra refrigeración', precioUnitario: 70000, cantidad: 1 }] },
    { ci: 7, vi: 7, desc: 'Cambio de bujías y revisión eléctrica', diag: 'Bujías con más de 40.000 km, ralentí inestable', mecanico: 1,
      items: [{ tipo: 'Producto', productoId: 4, cantidad: 1 }, { tipo: 'Servicio', nombre: 'Diagnóstico eléctrico', precioUnitario: 45000, cantidad: 1 }] },
  ];

  for (let i = 0; i < ordenes.length; i++) {
    const o = ordenes[i];
    const mecId = mecanicos.length > o.mecanico ? mecanicos[o.mecanico].id : null;
    const orden = await request('POST', '/Mantenimientos', {
      clienteId: clienteIds[o.ci],
      vehiculoId: vehiculoIds[o.vi],
      descripcion: o.desc,
      diagnostico: o.diag,
      mecanicoId: mecId
    });
    console.log(`  Orden ${orden.folio}: ${o.desc}`);

    for (const item of o.items) {
      const itemDto = item.tipo === 'Producto'
        ? { tipo: 'Producto', productoId: productoIds[item.productoId], cantidad: item.cantidad }
        : { tipo: 'Servicio', nombre: item.nombre, precioUnitario: item.precioUnitario, cantidad: item.cantidad };
      await request('POST', `/Mantenimientos/${orden.id}/items`, itemDto);
    }
    console.log(`    Items agregados: ${o.items.length}`);
  }

  console.log('\n=== SEED COMPLETADO ===');
}

main().catch(console.error);
