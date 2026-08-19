using IntugentBackend.Models;
using IntugentBackend.Services.Data;
using IntugentBackend.Services.Mfg;
using Microsoft.AspNetCore.Mvc;

namespace IntugentBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JetMixingController : ControllerBase
    {
        private readonly MfgJetMixing _mfgJetMixing;
        private readonly MfgPlantData _mfgPlantData;
        private readonly ILogger<JetMixingController> _logger;

        public JetMixingController(MfgJetMixing mfgJetMixing, MfgPlantData mfgPlantData, ILogger<JetMixingController> logger)
        {
            _mfgJetMixing = mfgJetMixing;
            _mfgPlantData = mfgPlantData;
            _logger = logger;
        }

        [HttpGet("data")]
        public IActionResult GetData()
        {
            try
            {
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Jet Mixing data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("lost-focus")]
        public IActionResult LostFocus([FromBody] JetFieldUpdateRequest request)
        {
            try
            {
                var jm = _mfgJetMixing.JetMix1;
                var conv = _mfgJetMixing.cUConv;
                bool hasValue = !string.IsNullOrEmpty(request.Value);
                double dum = hasValue ? double.Parse(request.Value!) : 0;

                switch (request.Name)
                {
                    case "gFRate_A": if (hasValue) jm.FRate_A = dum * conv.ToSi_FRate; break;
                    case "gTemp_A": if (hasValue) jm.Temp_A = (dum + 459.67) / 1.8; break;
                    case "gPres_A": if (hasValue) jm.Pres_A = dum * conv.ToSi_Pres; break;
                    case "gFRate_B": if (hasValue) jm.FRate_B = dum * conv.ToSi_FRate; break;
                    case "gTemp_B": if (hasValue) jm.Temp_B = (dum + 459.67) / 1.8; break;
                    case "gPres_B": if (hasValue) jm.Pres_B = dum * conv.ToSi_Pres; break;
                    case "gDens_A": if (hasValue) jm.Dens_A = dum * conv.ToSi_Dens; break;
                    case "gVisO_A": if (hasValue) jm.VisO_A = dum * conv.ToSi_Vis; break;
                    case "gVisE_A": if (hasValue) jm.VisE_A = dum; break;
                    case "gDens_B": if (hasValue) jm.Dens_B = dum * conv.ToSi_Dens; break;
                    case "gVisO_B": if (hasValue) jm.VisO_B = dum * conv.ToSi_Vis; break;
                    case "gVisE_B": if (hasValue) jm.VisE_B = dum; break;
                    case "gDiaMixChamb": if (hasValue) jm.DiaMixChamb = dum * conv.ToSi_Dia; break;
                    case "gDiaNoz_A": if (hasValue) jm.DiaNoz_A = dum * conv.ToSi_Dia; break;
                    case "gDiaNoz_B": if (hasValue) jm.DiaNoz_B = dum * conv.ToSi_Dia; break;
                    case "gPres_Max": if (hasValue) jm.Pres_Max = dum * conv.ToSi_Pres; break;
                    case "gPres_Min": if (hasValue) jm.Pres_Min = dum * conv.ToSi_Pres; break;
                    case "gReNo_Min": if (hasValue) jm.ReNo_Min = dum; break;
                    default: return BadRequest(new { success = false, error = $"Unknown field: {request.Name}" });
                }

                _mfgJetMixing.UpdateDataset();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Jet Mixing field {Field}", request.Name);
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        /// <summary>
        /// Optimizes Pres_A/Pres_B to center the jet impingement point (Zeta -&gt; 0) while keeping
        /// both jet Reynolds numbers reasonably above ReNo_Min, via a bounded gradient-descent search.
        /// </summary>
        [HttpPost("optimize")]
        public IActionResult Optimize()
        {
            try
            {
                var jm = _mfgJetMixing.JetMix1;
                double pi = Math.PI;

                jm.JetMixingSim();

                double tmpVel = jm.VolRate_A * 4.0 / (pi * jm.DiaNoz_A * jm.DiaNoz_A);
                double tmpPMin1 = 0.5 * jm.Dens_A * tmpVel * tmpVel;
                double tmpDia = jm.FRate_A * 4.0 / (pi * jm.Vis_A * jm.ReNo_Min);
                tmpVel = jm.VolRate_A * 4.0 / (pi * tmpDia * tmpDia);
                double tmpPMin2 = 0.5 * jm.Dens_A * tmpVel * tmpVel;
                double tmpPMinA = jm.Pres_Min;
                if (tmpPMin1 > tmpPMinA) tmpPMinA = tmpPMin1;
                if (tmpPMin2 > tmpPMinA) tmpPMinA = tmpPMin2;
                if (tmpPMinA > jm.Pres_Max) tmpPMinA = jm.Pres_Max;

                tmpVel = jm.VolRate_B * 4.0 / (pi * jm.Dens_B * jm.DiaNoz_B * jm.DiaNoz_B);
                tmpPMin1 = 0.5 * jm.Dens_B * tmpVel * tmpVel;
                tmpDia = jm.FRate_B * 4.0 / (pi * jm.Vis_B * jm.ReNo_Min);
                tmpVel = jm.VolRate_B * 4.0 / (pi * tmpDia * tmpDia);
                tmpPMin2 = 0.5 * jm.Dens_B * tmpVel * tmpVel;
                double tmpPMinB = jm.Pres_Min;
                if (tmpPMin1 > tmpPMinB) tmpPMinB = tmpPMin1;
                if (tmpPMin2 > tmpPMinB) tmpPMinB = tmpPMin2;
                if (tmpPMinB > jm.Pres_Max) tmpPMinB = jm.Pres_Max;

                var pMin = new double[] { tmpPMinA, tmpPMinB };
                var pMax = new double[] { jm.Pres_Max, jm.Pres_Max };

                if (jm.Pres_A < pMin[0]) jm.Pres_A = pMin[0];
                if (jm.Pres_B < pMin[1]) jm.Pres_B = pMin[1];
                if (jm.Pres_A > pMax[0]) jm.Pres_A = pMax[0];
                if (jm.Pres_B > pMax[1]) jm.Pres_B = pMax[1];

                var param = new double[] { jm.Pres_A, jm.Pres_B };
                int iter = 0;
                double fret = 0;

                double PFunc(double[] p)
                {
                    jm.Pres_A = p[0];
                    jm.Pres_B = p[1];
                    jm.JetMixingSim();
                    double dum = jm.Zeta * jm.Zeta;
                    if (jm.ReNo_A < 100.0) dum += 0.25 * (1 - 0.01 * jm.ReNo_A);
                    if (jm.ReNo_B < 100.0) dum += 0.25 * (1 - 0.01 * jm.ReNo_B);
                    return dum;
                }

                void DFunc(int n, double[] p, double[] dfp)
                {
                    var p1 = new double[n];
                    const double del = 0.001;
                    double fp = PFunc(p);
                    for (int ip = 0; ip < n; ip++)
                    {
                        for (int i = 0; i < n; i++) p1[i] = p[i];
                        double delp;
                        if (p[ip] < 0.1 && p[ip] > -0.1) { p1[ip] = del; delp = del; }
                        else { p1[ip] = (1.0 + del) * p[ip]; delp = (p1[ip] - p[ip]) / Math.Abs(p[ip]); }
                        double fp1 = PFunc(p1);
                        dfp[ip] = (fp1 - fp) / delp;
                    }
                }

                MyMathLib.Frprmn_1(param, pMax, pMin, 2, 0.0000001, ref iter, ref fret, PFunc, DFunc);

                // Note: matches legacy behavior — the optimized result is NOT persisted to session here.
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing Jet Mixing pressures");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("import")]
        public IActionResult Import([FromBody] JetImportRequest request)
        {
            try
            {
                if (!_mfgPlantData.GetDataSet())
                    return Ok(new { success = false, error = "Can not retrieve Pour Head Data" });

                var dr = _mfgPlantData.dr;
                var jm = _mfgJetMixing.JetMix1;
                var conv = _mfgJetMixing.cUConv;

                string n = request.Name switch
                {
                    "gImport1" => "1",
                    "gImport2" => "2",
                    "gImport3" => "3",
                    _ => string.Empty
                };
                if (n == string.Empty)
                    return BadRequest(new { success = false, error = $"Unknown import source: {request.Name}" });

                void ImportFlowRate(string col, Action<double> setter)
                {
                    if (dr[col] != DBNull.Value) setter((double)dr[col] * conv.ToSi_FRate);
                }
                void ImportTemp(string col, Action<double> setter)
                {
                    if (dr[col] != DBNull.Value) setter(((double)dr[col] + 459.67) / 1.8);
                }
                void ImportPres(string col, Action<double> setter)
                {
                    if (dr[col] != DBNull.Value) setter((double)dr[col] * conv.ToSi_Pres);
                }

                ImportFlowRate($"MDI {n} Pour Head Flowrate", v => jm.FRate_A = v);
                ImportFlowRate($"Poly {n} Pour Head Flowrate", v => jm.FRate_B = v);
                ImportTemp($"MDI {n} Pour Head Temperature", v => jm.Temp_A = v);
                ImportTemp($"Poly {n} Pour Head Temperature", v => jm.Temp_B = v);
                ImportPres($"MDI {n} Pour Head Pressure", v => jm.Pres_A = v);
                ImportPres($"Poly {n} Pour Head Pressure", v => jm.Pres_B = v);

                _mfgJetMixing.UpdateDataset();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing Pour Head data");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        [HttpPost("reset")]
        public IActionResult Reset()
        {
            try
            {
                _mfgJetMixing.SetDefaultValues();
                _mfgJetMixing.UpdateDataset();
                return Ok(new { success = true, data = BuildDto() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting Jet Mixing fields");
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }

        // ========== DTO BUILDER ==========

        private JetMixingDto BuildDto()
        {
            var jm = _mfgJetMixing.JetMix1;
            var conv = _mfgJetMixing.cUConv;

            jm.JetMixingSim();

            var dto = new JetMixingDto
            {
                GDiaMixChamb = (jm.DiaMixChamb / conv.ToSi_Dia).ToString("0.000"),
                GDiaNoz_A = (jm.DiaNoz_A / conv.ToSi_Dia).ToString("0.000"),
                GDiaNoz_B = (jm.DiaNoz_B / conv.ToSi_Dia).ToString("0.000"),
                GPres_Max = (jm.Pres_Max / conv.ToSi_Pres).ToString("0"),
                GPres_Min = (jm.Pres_Min / conv.ToSi_Pres).ToString("0"),
                GReNo_Min = jm.ReNo_Min.ToString("0.0"),

                GFRate_A = (jm.FRate_A / conv.ToSi_FRate).ToString("0.000"),
                GFRate_B = (jm.FRate_B / conv.ToSi_FRate).ToString("0.000"),
                GTemp_A = (jm.Temp_A * 1.8 - 459.67).ToString("0.0"),
                GTemp_B = (jm.Temp_B * 1.8 - 459.67).ToString("0.0"),
                GPres_A = (jm.Pres_A / conv.ToSi_Pres).ToString("0.0"),
                GPres_B = (jm.Pres_B / conv.ToSi_Pres).ToString("0.0"),
                GDens_A = (jm.Dens_A / conv.ToSi_Dens).ToString("0.00"),
                GDens_B = (jm.Dens_B / conv.ToSi_Dens).ToString("0.00"),
                GVisO_A = (jm.VisO_A / conv.ToSi_Vis).ToString("0.0"),
                GVisO_B = (jm.VisO_B / conv.ToSi_Vis).ToString("0.0"),
                GVisE_A = jm.VisE_A.ToString("0.0"),
                GVisE_B = jm.VisE_B.ToString("0.0")
            };

            string msg = string.Empty;
            if (jm.DiaJet_A > jm.DiaNoz_A) msg = "Nozzle A must be bigger than " + (jm.DiaJet_A / conv.ToSi_Dia).ToString("0.000E00") + " mm.  ";
            if (jm.DiaJet_B > jm.DiaNoz_B) msg += "Nozzle B must be bigger than " + (jm.DiaJet_B / conv.ToSi_Dia).ToString("0.000E00") + " mm";
            dto.GMsg = msg;

            dto.GDetails = new List<JetDetailRow>
            {
                new() { Description = "Jet Volume Flow Rate [m3/s]", JetA = jm.VolRate_A.ToString("0.000E00"), JetB = jm.VolRate_B.ToString("0.000E00") },
                new() { Description = "Fluid Viscosity [Pa-s]", JetA = jm.Vis_A.ToString("0.000E00"), JetB = jm.Vis_B.ToString("0.000E00") },
                new() { Description = "Jet Diameter [m]", JetA = jm.DiaJet_A.ToString("0.000E00"), JetB = jm.DiaJet_B.ToString("0.000E00") },
                new() { Description = "Jet Velocity [m/s]", JetA = jm.Vel_A.ToString("0.000E00"), JetB = jm.Vel_B.ToString("0.000E00") },
                new() { Description = "Jet Reynold No.", JetA = jm.ReNo_A.ToString("0.000E00"), JetB = jm.ReNo_B.ToString("0.000E00") },
                new() { Description = "Jet Kinetic Energy [J/s]", JetA = jm.KE_A.ToString("0.000E00"), JetB = jm.KE_B.ToString("0.000E00") },
                new() { Description = "Impingement Point [r/D]", JetA = jm.Zeta.ToString("0.000E00"), JetB = jm.Zeta.ToString("0.000E00") }
            };

            int nPts = MfgJetMixing.nPts;
            var xa = new double[nPts]; var ya = new double[nPts];
            var xb = new double[nPts]; var yb = new double[nPts];
            double dx = 1.0 / (nPts - 1);
            xa[0] = -0.5; xb[0] = 0.5; ya[0] = yb[0] = -0.5;
            for (int i = 1; i < nPts; i++)
            {
                ya[i] = ya[i - 1] + dx;
                yb[i] = ya[i];
                xa[i] = (jm.Zeta + 0.5) * (1 - 4.0 * ya[i] * ya[i]) - 0.5;
                xb[i] = 0.5 - (0.5 - jm.Zeta) * (1 - 4.0 * ya[i] * ya[i]);
            }
            // A zero/blank density, viscosity, etc. can drive the physics (sqrt/division) to
            // NaN/Infinity, which System.Text.Json throws on by default when serializing the
            // response — sanitize before returning so a bad input shows a flat/odd chart instead
            // of breaking the page (same class of issue found in the AI Model training endpoint).
            dto.XA = Sanitize(xa); dto.YA = Sanitize(ya); dto.XB = Sanitize(xb); dto.YB = Sanitize(yb);

            return dto;
        }

        private static double[] Sanitize(double[] values)
        {
            var result = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                double v = values[i];
                result[i] = double.IsNaN(v) || double.IsInfinity(v) ? 0 : v;
            }
            return result;
        }
    }
}
